using System.Collections.Generic;
using System.Linq;
using Game.General;
using Game.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Nodes
{
    public class Graph : MonoBehaviour
    {
        [SerializeField]
        private List<EdgeData> edges = new();
        
        public IReadOnlyDictionary<int, NodeView> NodeViews => _nodeViews;
        
        private readonly Dictionary<int, NodeView> _nodeViews = new();
        private readonly Dictionary<NodeType, List<NodeView>> _nodeTypeRelations = new();
        private readonly Dictionary<int, List<Neighbour>> _adjacencyMap = new();
        private readonly List<EdgeView> _edgeViews = new();

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
            
            var root = new GameObject { name = "Edges" };
            foreach (var edge in edges)
            {
                var edgeView = SpawnEdgeView(root.transform, edge);
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
            if (!_nodeViews.ContainsKey(startId) || !_nodeViews.ContainsKey(endId))
            {
                Debug.LogError("Wrong node ID");
                return new List<int>();
            }

            var distances = new Dictionary<int, int>();
            var previous = new Dictionary<int, int>();
            var visited = new HashSet<int>();
            var queue = new PriorityQueue<int, int>();
            
            //Init
            foreach (var id in _nodeViews.Keys)
            {
                distances[id] = int.MaxValue;
                previous[id] = -1;
            }

            distances[startId] = 0;
            queue.Enqueue(startId, 0);

            //Processing
            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                if (currentId == endId)
                    break;

                if (!visited.Add(currentId))
                    continue;

                if (!_adjacencyMap.TryGetValue(currentId, out var neighbours))
                {
                    Debug.LogError($"Couldn't find neighbours data in adjacency map for {currentId}");
                    return new List<int>();
                }
                
                foreach (var neighbour in neighbours)
                {
                    var neighbourId = neighbour.Node.ID;

                    if (visited.Contains(neighbourId))
                        continue;

                    var newDist = distances[currentId] + neighbour.Distance;
                    if (newDist >= distances[neighbourId]) 
                        continue;
                    
                    distances[neighbourId] = newDist;
                    previous[neighbourId] = currentId;
                    queue.Enqueue(neighbourId, newDist);
                }
            }

            //Result
            var result = new List<int>();
            var pathNodeId = endId;

            while (pathNodeId != -1)
            {
                result.Add(pathNodeId);
                pathNodeId = previous[pathNodeId];
            }

            result.Reverse();

            if (result[0] == startId) 
                return result;
            
            Debug.LogError("Path couldn't be found");
            return new List<int>();
        }
        
        public int FindNearest(int startBaseId, NodeType nodeType)
        {
            if (!_nodeViews.ContainsKey(startBaseId))
            {
                Debug.LogError($"Node ID {startBaseId} not found");
                return -1;
            }

            var distances = new Dictionary<int, int>();
            var visited = new HashSet<int>();
            var queue = new PriorityQueue<int, int>();

            foreach (var id in _nodeViews.Keys)
                distances[id] = int.MaxValue;

            distances[startBaseId] = 0;
            queue.Enqueue(startBaseId, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!visited.Add(current)) continue;

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
                    queue.Enqueue(neighborId, newDist);
                }
            }

            if (!_nodeTypeRelations.TryGetValue(nodeType, out var nodeViews))
            {
                Debug.LogError($"No node found with type {nodeType}");
                return -1;
            }

            var nearestNodeId = -1;
            var minDistance = int.MaxValue;

            foreach (var nodeView in nodeViews)
            {
                if (!distances.TryGetValue(nodeView.ID, out var dist) || dist >= minDistance) continue;
                
                minDistance = dist;
                nearestNodeId = nodeView.ID;
            }

            if (nearestNodeId == -1)
                Debug.LogWarning("No reachable nodes found");

            return nearestNodeId;
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

                    var baseNode = new BaseNode(Random.Range(1f, 10f));
                    baseNodeView?.Setup(baseNode);
                    break;
                }
                case NodeType.Mine:
                {
                    var mineNodeView = nodeView as MineNodeView;

                    var mineNode = new MineNode(Random.Range(0.1f, 1f));
                    mineNodeView?.Setup(mineNode);
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
        
        private EdgeView SpawnEdgeView(Transform root, EdgeData edgeData)
        {
            var prefab = Resources.Load<EdgeView>("Prefabs/Edge");
            var instance = Instantiate(prefab, root);
            instance.name = $"Edge_{edgeData.ID}";
            instance.Setup(edgeData);

            return instance;
        }

        private void UpdateAdjacencyMap()
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
        }
    }
}