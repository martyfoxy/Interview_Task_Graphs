using System;
using UnityEngine;

namespace Game.Views
{
    public class NodeListProvider : MonoBehaviour
    {
        [SerializeField]
        private NodeView[] _nodeViews;

        public NodeView[] NodeViews => _nodeViews;

        private void OnValidate()
        {
            _nodeViews = FindObjectsOfType<NodeView>(true);
        }
    }
}