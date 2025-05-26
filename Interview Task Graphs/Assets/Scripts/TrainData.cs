using Game.Interfaces;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "Create TrainData", fileName = "TrainData", order = 0)]
    public class TrainData : ScriptableObject, IResourceExtractor
    {
        [SerializeField, Range(1f, 1000f)]
        private float moveSpeed = 1f;

        [SerializeField, Range(1f, 100f)] 
        private float baseExtractionSpeed = 1f;

        public float MoveSpeed => moveSpeed;
        public float BaseExtractionSpeed => baseExtractionSpeed;
    }
}