using Game.General;
using Game.Interfaces;
using UnityEngine;

namespace Game.Views
{
    public class TrainView : ContextView<IResourceExtractor>, IResourceExtractor
    {
        [SerializeField]
        private GameObject extractionMarker;

        [SerializeField, Range(1f, 1000f)]
        private float moveSpeed;

        [SerializeField, Range(1f, 100f)]
        private float baseExtractionSpeed;

        public float MoveSpeed =>  moveSpeed;
        public float BaseExtractionSpeed => baseExtractionSpeed;
        
        protected override void Init()
        {
            extractionMarker.SetActive(false);
        }

        public override void Setup(IResourceExtractor context)
        {
            base.Setup(context);
            
            moveSpeed = context.MoveSpeed;
            baseExtractionSpeed = context.BaseExtractionSpeed;
        }

        public override void UpdateView()
        {
            
        }
    }
}