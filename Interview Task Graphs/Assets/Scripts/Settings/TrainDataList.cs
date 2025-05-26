using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(menuName = "Create TrainDataList", fileName = "TrainDataList", order = 0)]
    public class TrainDataList : ScriptableObject
    {
        [SerializeField]
        private List<TrainData> dataList;

        public List<TrainData> DataList => dataList;

        public TrainData GetRandom()
        {
            return dataList[Random.Range(0, dataList.Count)];
        }
    }
}