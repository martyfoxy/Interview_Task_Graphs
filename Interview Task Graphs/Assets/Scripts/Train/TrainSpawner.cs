using Game.Settings;
using Game.Nodes;
using Train;
using UnityEngine;

namespace Game.Train
{
    public class TrainSpawner
    {
        private readonly TrainDataList _trainDataList;
        private readonly Graph _graph;

        public TrainSpawner(TrainDataList trainDataList, Graph graph)
        {
            _trainDataList = trainDataList;
            _graph = graph;
        }

        public TrainView Spawn()
        {
            var prefab = Resources.Load<TrainView>("Prefabs/Train");

            var randomBase = _graph.GetRandomNodeOfType(NodeType.Base);
            
            var instance = Object.Instantiate(prefab, randomBase.transform.position, Quaternion.identity);

            var trainData = _trainDataList.GetRandomTrainData();
            instance.Setup(trainData, _graph);

            return instance;
        }
    }
}