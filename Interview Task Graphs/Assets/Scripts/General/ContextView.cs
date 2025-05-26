using Game.Interfaces;
using UnityEngine;

namespace Game.General
{
    public abstract class ContextView<T> : MonoBehaviour, IContextView<T>
    {
        private bool _alreadyInitialized;
        public T Context { get; protected set; }

        public virtual void Setup(T context)
        {
            Context = context;
			
            if (!_alreadyInitialized)
            {
                Init();
                _alreadyInitialized = true;
            }
			
            UpdateView();
        }

        public abstract void UpdateView();
        protected virtual void Init() { }
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void Update() { }
        protected virtual void LateUpdate() { }
        protected virtual void OnDestroy() { }
        public virtual void Dispose() { }
    }
}