using System;

namespace Game.Nodes
{
    [Serializable]
    public class Neighbour
    {
        public NodeView Node;
        public int Distance = 1;
    }
}