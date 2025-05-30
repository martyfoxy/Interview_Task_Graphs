using Game.General;
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
        [Range(Const.MineTimeMultiplierMin, Const.MineTimeMultiplierMax)]
        [Header("Changeable parameters")]
        private float timeMultiplier = Const.MineTimeMultiplierDefault;
        
        public float TimeMultiplier => timeMultiplier;

        public override NodeType NodeType => NodeType.Mine;

        protected override void SetupImpl()
        {
            timeMultiplier = Data.TimeMultiplier;
            multiplierText.text = $"x{timeMultiplier:F1}";
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!Application.isPlaying || Data == null) return;
            multiplierText.text = $"x{timeMultiplier:F1}";
        }
#endif
    }
}