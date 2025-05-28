using Game.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.Nodes
{
    public class EdgeView : MonoBehaviour, IEdgeData
    {
        [SerializeField]
        private TMP_Text weightText;
        
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private float weight;
        
        public float Weight => weight;

        public void Setup(Graph.EdgeData edgeData)
        {
            weight = edgeData.Weight;
            
            var startPos = edgeData.StartPos;
            var endPos = edgeData.EndPos;
            
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
            
            //TODO: Update text according to current weight
            weightText.text = weight.ToString("F1");
            weightText.transform.position = (startPos + endPos ) / 2;
        }
    }
}