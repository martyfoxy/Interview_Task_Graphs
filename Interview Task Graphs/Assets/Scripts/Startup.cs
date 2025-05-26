using System;
using System.Linq;
using Game.Settings;
using Game.Train;
using Game.Views;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private NodeListProvider nodeListProvider;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private TrainDataList trainDataList;

        private TrainSpawner _trainSpawner;
        
        private void Awake()
        {
            startButton.onClick.AddListener(OnStartClicked);
            _trainSpawner =  new TrainSpawner(trainDataList);
        }

        private void OnStartClicked()
        {
            startButton.onClick.RemoveListener(OnStartClicked);
            startButton.gameObject.SetActive(false);
            nodeListProvider.gameObject.SetActive(true);
            
            
            _trainSpawner.Spawn();
        }
    }   
}
