using Game.General;
using Game.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.Nodes
{
    /// <summary>
    /// Represents base
    /// </summary>
    public class BaseNodeView : NodeViewWithData<IResourceConsumer>, IResourceConsumer
    {
        [SerializeField]
        private TMP_Text multiplierText;
        
        [SerializeField]
        [Range(Const.BaseStorageMultiplierMin, Const.BaseStorageMultiplierMax)]
        [Header("Changeable parameters")]
        private float storageMultiplier = Const.BaseStorageMultiplierDefault;

        public float StorageMultiplier => storageMultiplier;

        public override NodeType NodeType => NodeType.Base;

        protected override void SetupImpl()
        {
            storageMultiplier = Data.StorageMultiplier;
            multiplierText.text = $"x{storageMultiplier:F1}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!Application.isPlaying || Data == null) return;
            multiplierText.text = $"x{storageMultiplier:F1}";
        }
#endif
    }
}