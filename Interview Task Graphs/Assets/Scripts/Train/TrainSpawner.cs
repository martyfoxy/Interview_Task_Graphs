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
        private readonly GameState _gameState;

        public TrainSpawner(TrainDataList trainDataList, Graph graph, GameState gameState)
        {
            _trainDataList = trainDataList;
            _graph = graph;
            _gameState = gameState;
        }

        public TrainView Spawn()
        {
            var prefab = Resources.Load<TrainView>("Prefabs/Train");

            var initialNode = _graph.GetRandomNode();
            
            var instance = Object.Instantiate(prefab, initialNode.transform.position, Quaternion.identity);

            var trainData = _trainDataList.GetRandomTrainData();
            instance.Setup(trainData, initialNode, _graph, _gameState);

            return instance;
        }
    }
}