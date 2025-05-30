using Game.General;
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
        private readonly ResourcesManager _resourcesManager;

        public TrainSpawner(TrainDataList trainDataList, Graph graph, ResourcesManager resourcesManager)
        {
            _trainDataList = trainDataList;
            _graph = graph;
            _resourcesManager = resourcesManager;
        }

        public TrainView Spawn()
        {
            var prefab = Resources.Load<TrainView>(Const.TrainViewPrefabPath);

            var initialNode = _graph.GetRandomNode();
            
            var instance = Object.Instantiate(prefab, initialNode.transform.position, Quaternion.identity);

            var trainData = _trainDataList.GetRandomTrainData();
            instance.Setup(trainData, initialNode, _graph, _resourcesManager);

            return instance;
        }
    }
}