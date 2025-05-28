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
        [Range(1f, 5f)]
        [Header("Changeable parameters")]
        private float storageMultiplier;

        public float StorageMultiplier => storageMultiplier;

        public override NodeType NodeType => NodeType.Base;

        public override void Setup(IResourceConsumer data)
        {
            storageMultiplier = data.StorageMultiplier;
            multiplierText.text = data.StorageMultiplier.ToString("F1");
            //TODO: Сделать обновление UI при изменении 
        }
    }
}