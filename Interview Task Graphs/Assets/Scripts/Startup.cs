using Game.Settings;
using Game.Train;
using Game.Nodes;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Entry point of the game
    /// </summary>
    public class Startup : MonoBehaviour
    {
        private const int TrainsCount = 1;
        
        [SerializeField]
        private Graph graph;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private TrainDataList trainDataList;

        private TrainSpawner _trainSpawner;
        
        private void Awake()
        {
            startButton.onClick.AddListener(OnStartClicked);
            _trainSpawner =  new TrainSpawner(trainDataList, graph);
        }

        private void OnStartClicked()
        {
            startButton.onClick.RemoveListener(OnStartClicked);
            startButton.gameObject.SetActive(false);
            graph.gameObject.SetActive(true);
            
            graph.Init();
            for (var i = 0; i < TrainsCount; i++)
            {
                var train = _trainSpawner.Spawn();
                train.Think();
            }
        }
    }   
}
