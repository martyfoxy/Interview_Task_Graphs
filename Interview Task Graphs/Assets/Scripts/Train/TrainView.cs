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
        [Range(Const.TrainSpeedMin, Const.TrainSpeedMax)]
        [Header("Changeable parameters")]
        private float moveSpeed = Const.TrainSpeedDefault;

        [SerializeField]
        [Range(Const.TrainExtractionSpeedMin, Const.TrainExtractionSpeedMax)]
        private float baseExtractionSpeed = Const.TrainExtractionSpeedDefault;

        private Graph _graph;
        private ResourcesManager _resourcesManager;
        private int _currentNodeId;
        private List<int> _currentPath;

        public float MoveSpeed =>  moveSpeed;
        public float BaseExtractionSpeed => baseExtractionSpeed;
        
        public void Setup(IResourceExtractor context, NodeView initialNode, Graph graph, ResourcesManager resourcesManager)
        {
            extractionMarker.SetActive(false);
            resourceMarker.SetActive(false);
            
            moveSpeed = context.MoveSpeed;
            baseExtractionSpeed = context.BaseExtractionSpeed;
            
            _currentNodeId = initialNode.ID;
            _graph = graph;
            _resourcesManager = resourcesManager;
        }

        public IEnumerator Think()
        {
            while (true)
            {
                var bestMine = FindBestMine(_currentNodeId);
                yield return MovingToMine(_currentNodeId, bestMine);
                
                yield return ExtractingResource(bestMine);

                var bestBase = FindBestBase(bestMine);
                yield return MovingToBaseWithResource(bestMine, bestBase);   
            }
        }

        private int FindBestMine(int fromNodeId)
        {
            if (!_graph.TryRunDijkstra(fromNodeId, out var distances, out _))
                return -1;

            if (!_graph.NodeTypeRelations.TryGetValue(NodeType.Mine, out var mines))
            {
                Debug.LogError("No mines found in graph");
                return -1;
            }

            var bestMineId = -1;
            var bestValue = float.MaxValue;

            foreach (var mineNode in mines)
            {
                if (mineNode is not MineNodeView mine) continue;

                if (!distances.TryGetValue(mine.ID, out int distance)) continue;

                var adjustedMiningTime = baseExtractionSpeed * mine.TimeMultiplier;
                var cost = distance + adjustedMiningTime;

                if (cost < bestValue)
                {
                    bestValue = cost;
                    bestMineId = mine.ID;
                }
            }

            return bestMineId;
        }

        private int FindBestBase(int fromNodeId)
        {
            if (!_graph.TryRunDijkstra(fromNodeId, out var distances, out _))
                return -1;

            if (!_graph.NodeTypeRelations.TryGetValue(NodeType.Base, out var bases))
            {
                Debug.LogError("No bases found in graph");
                return -1;
            }

            var bestBaseId = -1;
            var bestValue = float.MaxValue;

            foreach (var baseNode in bases)
            {
                if (baseNode is not BaseNodeView baseView) continue;

                if (!distances.TryGetValue(baseView.ID, out var distance)) continue;

                var cost = distance / Mathf.Max(baseView.StorageMultiplier, 0.01f);

                if (cost < bestValue)
                {
                    bestValue = cost;
                    bestBaseId = baseView.ID;
                }
            }

            return bestBaseId;
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

            if (targetNode is not IResourceProducer resourceProducer)
                yield break;

            yield return new WaitForSeconds(baseExtractionSpeed * resourceProducer.TimeMultiplier);

            extractionMarker.SetActive(false);
            resourceMarker.SetActive(true);
        }
        
        private IEnumerator MovingToBaseWithResource(int fromNodeId, int baseNodeId)
        {
            if (!_graph.NodeViews.TryGetValue(baseNodeId, out var targetNode))
                yield break;
            
            extractionMarker.SetActive(false);
            resourceMarker.SetActive(true);

            if (targetNode is not IResourceConsumer resourceConsumer)
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
            
            var score = 1f * resourceConsumer.StorageMultiplier;
            _resourcesManager.AddScore(score);
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