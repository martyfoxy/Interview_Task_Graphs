using System.Collections.Generic;
using System.Linq;
using Game.General;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Nodes
{
    /// <summary>
    /// Graph. It takes all the node views created on the scene and makes a graph out of it.
    /// Uses dijkstra algorithm to calculate shortest paths 
    /// </summary>
    public class Graph : MonoBehaviour
    {
        [SerializeField]
        private List<EdgeData> edges = new();
        
        public IReadOnlyDictionary<int, NodeView> NodeViews => _nodeViews;
        public IReadOnlyDictionary<NodeType, List<NodeView>> NodeTypeRelations => _nodeTypeRelations;
        
        private readonly Dictionary<int, NodeView> _nodeViews = new();
        private readonly Dictionary<NodeType, List<NodeView>> _nodeTypeRelations = new();
        private readonly Dictionary<int, List<Neighbour>> _adjacencyMap = new();
        private readonly List<EdgeView> _edgeViews = new();
        private GameObject _edgesRoot;

        public void Init()
        {
            edges.Clear();
            
            var allNodes = FindObjectsOfType<NodeView>(true);

            foreach (var nodeView in allNodes)
            {
                RegisterNodeView(nodeView);

                foreach (var neighbour in nodeView.Neighbours)
                {
                    var edge = CreateEdge(nodeView, neighbour);
                    edges.Add(edge);
                }
            }
            
            UpdateAdjacencyMap();
            
            _edgesRoot = new GameObject { name = "Edges" };
            foreach (var edge in edges)
            {
                var edgeView = SpawnEdgeView(edge);
                _edgeViews.Add(edgeView);
            }
            
            Debug.Log($"Graph initialized. Nodes: {allNodes.Length} Edges: {edges.Count}");
        }
        
        public NodeView GetRandomNode()
        {
            var nodeView = _nodeViews.Values.ElementAtOrDefault(Random.Range(0, _nodeViews.Count));
            return nodeView;
        }
        
        public List<int> FindShortestPath(int startId, int endId)
        {
            if (!TryRunDijkstra(startId, out var distances, out var previousSteps))
            {
                Debug.LogError("Dijkstra failed to run");
                return new List<int>();
            }

            if (!previousSteps.ContainsKey(endId) || distances[endId] == int.MaxValue)
            {
                Debug.LogError("Path couldn't be found");
                return new List<int>();
            }

            var path = new List<int>();
            for (var i = endId; i != -1; i = previousSteps[i])
                path.Add(i);

            path.Reverse();
            return path;
        }
        
        public bool TryRunDijkstra(int startId, out Dictionary<int, int> distances, out Dictionary<int, int> previousSteps)
        {
            distances = new Dictionary<int, int>();
            previousSteps = new Dictionary<int, int>();
            var visited = new HashSet<int>();
            var queue = new PriorityQueue<int, int>();

            if (!_nodeViews.ContainsKey(startId))
            {
                Debug.LogError($"Invalid start node ID: {startId}");
                return false;
            }

            foreach (var id in _nodeViews.Keys)
            {
                distances[id] = int.MaxValue;
                previousSteps[id] = -1;
            }

            distances[startId] = 0;
            queue.Enqueue(startId, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!visited.Add(current))
                    continue;

                if (!_adjacencyMap.TryGetValue(current, out var neighbours))
                    continue;

                foreach (var neighbour in neighbours)
                {
                    if (neighbour.Node == null) continue;

                    var neighborId = neighbour.Node.ID;
                    if (visited.Contains(neighborId)) continue;

                    var newDist = distances[current] + neighbour.Distance;
                    if (newDist >= distances[neighborId]) continue;

                    distances[neighborId] = newDist;
                    previousSteps[neighborId] = current;
                    queue.Enqueue(neighborId, newDist);
                }
            }

            return true;
        }
        
        private void RegisterNodeView(NodeView nodeView)
        {
            _nodeViews.Add(nodeView.ID, nodeView);

            if (_nodeTypeRelations.TryGetValue(nodeView.NodeType, out var nodeViews))
                nodeViews.Add(nodeView);
            else
                _nodeTypeRelations.Add(nodeView.NodeType, new List<NodeView> { nodeView });

            switch (nodeView.NodeType)
            {
                case NodeType.Base:
                {
                    var baseNodeView = nodeView as BaseNodeView;

                    var baseNode = new BaseNode(Random.Range(Const.BaseStorageMultiplierMin, Const.BaseStorageMultiplierMax));
                    baseNodeView?.Setup(baseNode, this);
                    break;
                }
                case NodeType.Mine:
                {
                    var mineNodeView = nodeView as MineNodeView;

                    var mineNode = new MineNode(Random.Range(Const.MineTimeMultiplierMin, Const.MineTimeMultiplierMax));
                    mineNodeView?.Setup(mineNode, this);
                    break;
                }
            }
        }

        private EdgeData CreateEdge(NodeView startNode, Neighbour neighbour)
        {
            var nodeId = startNode.ID;
            var nodePos = startNode.transform.position;
            
            if (neighbour.Node == null) return null;
                    
            var otherId = neighbour.Node.ID;
            var otherPos = neighbour.Node.transform.position;
            var weight = neighbour.Distance;
            
            var edge = new EdgeData(nodeId, otherId, nodePos, otherPos, weight);
            return edge;
        }
        
        private EdgeView SpawnEdgeView(EdgeData edgeData)
        {
            var prefab = Resources.Load<EdgeView>(Const.EdgeViewPrefabPath);
            var instance = Instantiate(prefab, _edgesRoot.transform);
            instance.name = $"Edge_{edgeData.ID}";
            instance.Setup(edgeData);

            return instance;
        }

        [ContextMenu("Update Adjacency Map")]
        public void UpdateAdjacencyMap()
        {
            _adjacencyMap.Clear();
            
            foreach (var (nodeId, nodeView) in _nodeViews)
            {
                foreach (var neighbour in nodeView.Neighbours)
                {
                    if (neighbour.Node == null) continue;
                    
                    var neighbourId = neighbour.Node.ID;

                    if (!_adjacencyMap.TryGetValue(nodeId, out var directList))
                        _adjacencyMap[nodeId] = directList = new List<Neighbour>();
                    directList.Add(neighbour);

                    if (!_adjacencyMap.TryGetValue(neighbourId, out var indirectList))
                        _adjacencyMap[neighbourId] = indirectList = new List<Neighbour>();

                    if (indirectList.All(n => n.Node.ID != nodeView.ID))
                    {
                        indirectList.Add(new Neighbour { Node = nodeView, Distance = neighbour.Distance });
                    }
                }   
            }

#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            if (_edgesRoot == null) return;
            
            foreach (var edge in _edgeViews)
            {
                if (edge != null) 
                    Destroy(edge.gameObject);
            }
            _edgeViews.Clear();
            
            foreach (var (fromId, neighbours) in _adjacencyMap)
            {
                var fromNodeView = _nodeViews[fromId];
                foreach (var neighbour in neighbours)
                {
                    var edgeData = CreateEdge(fromNodeView, neighbour);
                    var edgeView = SpawnEdgeView(edgeData);
                    _edgeViews.Add(edgeView);
                }
            }
#endif
        }
    }
}