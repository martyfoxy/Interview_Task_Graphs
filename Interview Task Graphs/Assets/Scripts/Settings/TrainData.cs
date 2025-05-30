using Game.General;
using Game.Interfaces;
using UnityEngine;

namespace Game.Settings
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "Create TrainData", fileName = "TrainData", order = 0)]
    public class TrainData : ScriptableObject, IResourceExtractor
    {
        [SerializeField, Range(Const.TrainSpeedMin, Const.TrainSpeedMax)]
        private float moveSpeed = Const.TrainSpeedDefault;

        [SerializeField, Range(Const.TrainExtractionSpeedMin, Const.TrainExtractionSpeedMax)] 
        private float baseExtractionSpeed = Const.TrainExtractionSpeedDefault;

        public float MoveSpeed => moveSpeed;
        public float BaseExtractionSpeed => baseExtractionSpeed;
    }
}