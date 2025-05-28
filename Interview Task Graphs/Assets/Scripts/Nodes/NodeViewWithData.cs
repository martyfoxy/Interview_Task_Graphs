namespace Game.Nodes
{
    public abstract class NodeViewWithData<T> : NodeView
    {
        public abstract override NodeType NodeType { get; }
        public abstract void Setup(T data);
    }
}