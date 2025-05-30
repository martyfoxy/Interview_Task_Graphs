using System;
using Game.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Nodes
{
    [Serializable]
    public class EdgeData : IEdgeData
    {
        [HideInInspector]
        public int ID;
        
        [HideInInspector]
        public Vector3 StartPos;
            
        [HideInInspector]
        public Vector3 EndPos;
            
        public int Weight { get; private set; }

        public EdgeData(int startNodeID, int endNodeID, Vector3 startPos, Vector3 endPos, int weight)
        {
            ID = Random.Range(int.MinValue, int.MaxValue);
            StartPos = startPos;
            EndPos = endPos;
            Weight = weight;
        }
    }
}