using Game.Settings;
using Game.Train;
using Game.Nodes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Entry point of the game
    /// </summary>
    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private Graph graph;

        [SerializeField]
        private Button startButton;
        
        [SerializeField]
        private TMP_InputField inputField;
        
        [SerializeField]
        private TMP_Text resourcesCountText;

        [SerializeField]
        private TrainDataList trainDataList;

        private ResourcesManager _resourcesManager;
        private TrainSpawner _trainSpawner;
        
        private void Awake()
        {
            startButton.onClick.AddListener(OnStartClicked);
            _resourcesManager = new ResourcesManager(resourcesCountText);
            _trainSpawner =  new TrainSpawner(trainDataList, graph, _resourcesManager);
        }

        private void OnStartClicked()
        {
            startButton.onClick.RemoveListener(OnStartClicked);
            startButton.gameObject.SetActive(false);
            inputField.gameObject.SetActive(false);
            graph.gameObject.SetActive(true);
            
            graph.Init();
            var trainsCount = int.Parse(inputField.text);
            for (var i = 0; i < trainsCount; i++)
            {
                var train = _trainSpawner.Spawn();
                StartCoroutine(train.Think());
            }
        }
    }
}
