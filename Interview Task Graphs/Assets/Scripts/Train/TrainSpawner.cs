using Game.Settings;
using Game.Views;
using UnityEngine;

namespace Game.Train
{
    public class TrainSpawner
    {
        private readonly TrainDataList _trainDataList;

        public TrainSpawner(TrainDataList trainDataList)
        {
            _trainDataList = trainDataList;
        }

        public TrainView Spawn()
        {
            var prefab = Resources.Load<TrainView>("Prefabs/Train");
            var instance = Object.Instantiate(prefab, new Vector3(), Quaternion.identity);

            var data = _trainDataList.GetRandom();
            instance.Setup(data);

            return instance;
        }
    }
}