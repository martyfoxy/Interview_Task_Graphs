using UnityEngine;

namespace Game.Nodes
{
    public abstract class NodeViewWithData<T> : NodeView
    {
        public abstract override NodeType NodeType { get; }
        protected T Data;
        private Graph _graph;

        public void Setup(T data, Graph graph)
        {
            Data = data;
            _graph = graph;
            SetupImpl();
        }

        protected abstract void SetupImpl();
        
        //This lets us update adjacency map in editor after we make changes to graph (e.g. when we change weights) 
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!Application.isPlaying) return;
            _graph?.UpdateAdjacencyMap();
        }
#endif
    }
}