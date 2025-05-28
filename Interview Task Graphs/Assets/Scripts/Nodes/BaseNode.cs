using Game.Interfaces;

namespace Game.Nodes
{
    public class BaseNode : IResourceConsumer
    {
        public float StorageMultiplier { get; }

        public BaseNode(float storageMultiplier)
        {
            StorageMultiplier = storageMultiplier;
        }
    }
}