using System;
using UnityEngine;

namespace Game.Nodes
{
    [Serializable]
    public class Neighbour
    {
        public NodeView Node;
        
        [Min(1)]
        public int Distance = 1;
    }
}