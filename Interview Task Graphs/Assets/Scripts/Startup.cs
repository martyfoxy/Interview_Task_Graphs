using System;
using System.Linq;
using Game.Train;
using Game.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private NodeListProvider nodeListProvider;

        [SerializeField]
        private Button startButton;

        private TrainSpawner _trainSpawner;
        
        private void Awake()
        {
            startButton.onClick.AddListener(OnStartClicked);
            _trainSpawner =  new TrainSpawner();
        }

        private void OnStartClicked()
        {
            foreach (var nodeView in nodeListProvider.NodeViews)
            {
                
            }
            
            _trainSpawner.Spawn(nodeListProvider.NodeViews.FirstOrDefault());
        }
    }   
}
