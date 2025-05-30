using Game.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.Nodes
{
    /// <summary>
    /// Represents mine
    /// </summary>
    public class MineNodeView : NodeViewWithData<IResourceProducer>, IResourceProducer
    {
        [SerializeField]
        private TMP_Text multiplierText;

        [SerializeField]
        [Range(0.01f, 1f)]
        [Header("Changeable parameters")]
        private float timeMultiplier = 1f;
        
        public float TimeMultiplier => timeMultiplier;

        public override NodeType NodeType => NodeType.Mine;

        public override void Setup(IResourceProducer data)
        {
            timeMultiplier = data.TimeMultiplier;
            multiplierText.text = $"x{data.TimeMultiplier:F1}";
            //TODO: Сделать обновление UI при изменении 
        }
    }
}