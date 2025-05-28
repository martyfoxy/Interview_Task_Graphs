using System;
using System.Collections.Generic;
using System.Linq;
using Game.General;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.Nodes
{
    public class Graph : MonoBehaviour
    {
        [SerializeField]
        private List<EdgeData> edges = new();
        
        public IReadOnlyDictionary<int, NodeView> NodeViews => _nodeViews;
        public IReadOnlyDictionary<NodeType, Dictionary<int, NodeView>>  NodeTypeRelations => _nodeTypeRelations;
        
        private readonly HashSet<(int, int)> _processedEdges = new();
        private readonly Dictionary<int, NodeView> _nodeViews = new();
        private readonly Dictionary<NodeType, Dictionary<int, NodeView>> _nodeTypeRelations = new();
        
        public void Init()
        {
            edges.Clear();
            _processedEdges.Clear();
            
            var allNodes = FindObjectsOfType<NodeView>(true);

            foreach (var nodeView in allNodes)
            {
                RegisterNodeView(nodeView);
                
                var nodeId = nodeView.ID;
                var nodePos = nodeView.transform.position;
                
                foreach (var neighbour in nodeView.Neighbours)
                {
                    if (neighbour.Node == null) continue;
                    
                    var otherId = neighbour.Node.ID;
                    var otherPos = neighbour.Node.transform.position;
                    var weight = neighbour.Distance;
                    
                    var orderedPair = nodeId < otherId ? (nodeId, otherId) : (otherId, nodeId);

                    if (_processedEdges.Contains(orderedPair)) continue;
                    
                    var edge = new EdgeData(nodeId, otherId, nodePos, otherPos, weight);
                        
                    edges.Add(edge);
                    _processedEdges.Add(orderedPair);
                }
            }

            var root = new GameObject
            {
                name = "Edges"
            };
            
            foreach (var edge in edges)
            {
                SpawnLine(root.transform, edge);
            }
            
            Debug.Log($"Graph initialized. Nodes: {allNodes.Length} Edges: {edges.Count}");
        }

        
        
        public NodeView GetRandomNodeOfType(NodeType nodeType)
        {
            if (!_nodeTypeRelations.TryGetValue(nodeType, out var entries))
            {
                Debug.LogError($"NodeViews with NodeType {nodeType} not found");
                return null;
            }

            var nodeView = entries.Values.ElementAtOrDefault(Random.Range(0, entries.Count));
            return nodeView;
        }

        public NodeView GetRandomNode()
        {
            return _nodeViews.Values.ElementAtOrDefault(Random.Range(0, _nodeViews.Count));
        }
        
        public List<int> FindShortestPath(int startId, int endId)
        {
            if (!_nodeViews.ContainsKey(startId) || !_nodeViews.ContainsKey(endId))
            {
                Debug.LogError("Wrong node ID");
                return new List<int>();
            }

            var distances = new Dictionary<int, float>();
            var previous = new Dictionary<int, int>();
            var visited = new HashSet<int>();
            var queue = new PriorityQueue<int, float>();
            
            //Init
            foreach (var id in _nodeViews.Keys)
            {
                distances[id] = float.MaxValue;
                previous[id] = -1;
            }

            distances[startId] = 0f;
            queue.Enqueue(startId, 0f);

            //Processing
            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                if (currentId == endId)
                    break;

                if (!visited.Add(currentId))
                    continue;

                //TODO: Optimize
                foreach (var edge in edges)
                {
                    var neighborId = -1;
                    var weight = edge.Weight;

                    if (edge.StartNodeId == currentId)
                        neighborId = edge.EndNodeId;
                    else if (edge.EndNodeId == currentId)
                        neighborId = edge.StartNodeId;
                    else
                        continue;

                    if (visited.Contains(neighborId))
                        continue;

                    var newDist = distances[currentId] + weight;
                    if (newDist < distances[neighborId])
                    {
                        distances[neighborId] = newDist;
                        previous[neighborId] = currentId;
                        queue.Enqueue(neighborId, newDist);
                    }
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

            if (result[0] == startId) return result;
            
            Debug.LogError("Path couldn't be found");
            return new List<int>();
        }

        public NodeView GetClosestNode(Vector3 worldPos, NodeType nodeType)
        {
            return null;
        }
        
        private void RegisterNodeView(NodeView nodeView)
        {
            _nodeViews.Add(nodeView.ID, nodeView);
                
            if (_nodeTypeRelations.TryGetValue(nodeView.NodeType, out var entries))
                entries.Add(nodeView.ID, nodeView);
            else
                _nodeTypeRelations.Add(nodeView.NodeType, new Dictionary<int, NodeView> { { nodeView.ID, nodeView } });
        }
        
        private void SpawnLine(Transform root, EdgeData edgeData)
        {
            var prefab = Resources.Load<EdgeView>("Prefabs/Edge");
            var instance = Instantiate(prefab, root);
            instance.name = $"Edge_{edgeData.ID}";
            
            instance.Setup(edgeData);
        }

        [Serializable]
        public class EdgeData : IEdgeData
        {
            [HideInInspector]
            public int ID;
            
            [HideInInspector]
            public int StartNodeId;
            
            [HideInInspector]
            public Vector3 StartPos;
            
            [HideInInspector]
            public Vector3 EndPos;
            
            [HideInInspector]
            public int EndNodeId;
            
            public float Weight { get; private set; }

            public EdgeData(int startNodeID, int endNodeID, Vector3 startPos, Vector3 endPos, float weight)
            {
                ID = Random.Range(int.MinValue, int.MaxValue);
                StartNodeId = startNodeID;
                EndNodeId = endNodeID;
                StartPos = startPos;
                EndPos = endPos;
                Weight = weight;
            }
        }
    }
}