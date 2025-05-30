using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.General;
using Game.Interfaces;
using Game.Nodes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Train
{
    public class TrainView : MonoBehaviour, IResourceExtractor
    {
        [SerializeField]
        private GameObject extractionMarker;

        [SerializeField]
        private GameObject resourceMarker;

        [SerializeField]
        [Range(1f, 1000f)]
        [Header("Changeable parameters")]
        private float moveSpeed;

        [SerializeField]
        [Range(1f, 100f)]
        private float baseExtractionSpeed;

        private Graph _graph;
        private GameState _gameState;
        private int _currentNodeId;
        private List<int> _currentPath;

        public float MoveSpeed =>  moveSpeed;
        public float BaseExtractionSpeed => baseExtractionSpeed;
        
        public void Setup(IResourceExtractor context, NodeView initialNode, Graph graph, GameState gameState)
        {
            extractionMarker.SetActive(false);
            resourceMarker.SetActive(false);
            
            moveSpeed = context.MoveSpeed;
            baseExtractionSpeed = context.BaseExtractionSpeed;
            
            _currentNodeId = initialNode.ID;
            _graph = graph;
            _gameState = gameState;
        }

        public IEnumerator Think()
        {
            while (true)
            {
                var nearestMineId = _graph.FindNearest(_currentNodeId, NodeType.Mine);
                Debug.Log(">>> Moving to nearest Mine: " + nearestMineId);
                yield return MovingToMine(_currentNodeId, nearestMineId);
            
                Debug.Log(">>> Extracting...");
                yield return ExtractingResource(nearestMineId);
                Debug.Log(">>> Extracting completed");
            
                var nearestBaseId = _graph.FindNearest(_currentNodeId, NodeType.Base);
                Debug.Log(">>> Moving to nearest Base: " + nearestBaseId);
                yield return MovingToBaseWithResource(_currentNodeId, nearestBaseId);   
            }
        }

        private IEnumerator MovingToMine(int fromNodeId, int mineId)
        {
            _currentPath = _graph.FindShortestPath(fromNodeId, mineId);
            
            if (_currentPath == null || _currentPath.Count == 0)
                yield break;

            foreach (var nodeId in _currentPath)
            {
                if (!_graph.NodeViews.TryGetValue(nodeId, out var pathNode))
                    continue;

                yield return MoveToPosition(pathNode.transform.position);
            }

            _currentNodeId = mineId;
        }
        
        private IEnumerator ExtractingResource(int mineNodeId)
        {
            if (!_graph.NodeViews.TryGetValue(mineNodeId, out var targetNode))
                yield break;
            
            extractionMarker.SetActive(true);
            resourceMarker.SetActive(false);

            var resourceProducer = targetNode as IResourceProducer;
            if (resourceProducer == null)
                yield break;

            yield return new WaitForSeconds(baseExtractionSpeed / resourceProducer.TimeMultiplier);

            extractionMarker.SetActive(false);
            resourceMarker.SetActive(true);
        }
        
        private IEnumerator MovingToBaseWithResource(int fromNodeId, int baseNodeId)
        {
            if (!_graph.NodeViews.TryGetValue(baseNodeId, out var targetNode))
                yield break;
            
            extractionMarker.SetActive(false);
            resourceMarker.SetActive(true);

            var resourceConsumer = targetNode as IResourceConsumer;
            if (resourceConsumer == null)
                yield break;
            
            _currentPath = _graph.FindShortestPath(fromNodeId, baseNodeId);
            if (_currentPath == null || _currentPath.Count == 0)
                yield break;

            foreach (var nodeId in _currentPath)
            {
                if (!_graph.NodeViews.TryGetValue(nodeId, out var pathNode))
                    continue;

                yield return MoveToPosition(pathNode.transform.position);
            }
            
            extractionMarker.SetActive(false);
            resourceMarker.SetActive(false);

            _currentNodeId = baseNodeId;
            
            var score = (int)(1 * resourceConsumer.StorageMultiplier);
            _gameState.AddScore(score);
        }
        
        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPosition;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_currentPath == null || _graph == null)
                return;

            NodeView previous = null;
            foreach (var nodeId in _currentPath)
            {
                if (!_graph.NodeViews.TryGetValue(nodeId, out var pathNode))
                    continue;

                if (previous != null)
                {
                    Handles.color = Color.green;
                    Handles.DrawAAPolyLine(20f, previous.transform.position, pathNode.transform.position);
                }

                previous = pathNode;
            }
        }
#endif
    }
}