using System;
using UnityEngine;

namespace Game.Views
{
    public class NodeListProvider : MonoBehaviour
    {
        [SerializeField]
        private NodeView[] nodeViews;

        public NodeView[] NodeViews => nodeViews;

        private void OnValidate()
        {
            nodeViews = FindObjectsOfType<NodeView>(true);
        }
    }
}