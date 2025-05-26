using Game.Views;
using UnityEngine;

namespace Game.Train
{
    public class TrainSpawner
    {
        public TrainSpawner()
        {
            
        }

        public TrainView Spawn(NodeView node)
        {
            if(node == null) return null;
            
            var prefab = Resources.Load<TrainView>("Prefabs/Train");
            var instance = Object.Instantiate(prefab, node.transform.position, Quaternion.identity);

            return instance;
        }
    }
}