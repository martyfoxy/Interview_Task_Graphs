using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.General;
using Game.Interfaces;
using Game.Nodes;
using UnityEngine;

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

        public float MoveSpeed =>  moveSpeed;
        public float BaseExtractionSpeed => baseExtractionSpeed;
        
        public void Setup(IResourceExtractor context, Graph graph)
        {
            extractionMarker.SetActive(false);
            resourceMarker.SetActive(false);
            
            moveSpeed = context.MoveSpeed;
            baseExtractionSpeed = context.BaseExtractionSpeed;
            
            _graph = graph;
        }

        public void Think()
        {
            var closestMine = _graph.GetClosestNode(transform.position, NodeType.Mine);
            
            //var path = _graph.FindShortestPath(randomBase.ID, randomMine.ID);
            //StartCoroutine(Moving(path));
        }

        private IEnumerator Moving(List<int> path)
        {
            if (path == null || path.Count == 0)
                yield break;

            foreach (var nodeId in path)
            {
                if (!_graph.NodeViews.TryGetValue(nodeId, out var targetNode))
                    continue;

                yield return MoveToPosition(targetNode.transform.position);
                
                if (targetNode is MineNodeView mine)
                {
                    extractionMarker.SetActive(true);
                    yield return new WaitForSeconds(baseExtractionSpeed);
                    extractionMarker.SetActive(false);
                }
            }
            
            Think();
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
    }
}