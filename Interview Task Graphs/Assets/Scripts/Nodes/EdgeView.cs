using Game.General;
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
        private int weight;
        
        public int Weight => weight;

        public void Setup(EdgeData edgeData)
        {
            weight = edgeData.Weight;
            
            var startPos = edgeData.StartPos;
            var endPos = edgeData.EndPos;
            
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
            
            var t = Mathf.InverseLerp(Const.MinDistance, Const.MaxDistance, weight);
            var edgeColor = Color.Lerp(Color.green, Color.red, t);
            lineRenderer.startColor = lineRenderer.endColor = edgeColor;
            
            weightText.text = weight.ToString();
            weightText.transform.position = (startPos + endPos ) * 0.5f;
        }
    }
}