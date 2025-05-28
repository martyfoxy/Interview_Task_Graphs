using Game.Interfaces;

namespace Game.Nodes
{
    public class MineNode : IResourceProducer
    {
        public float TimeMultiplier { get; }

        public MineNode(float timeMultiplier)
        {
            TimeMultiplier = timeMultiplier;
        }
    }
}