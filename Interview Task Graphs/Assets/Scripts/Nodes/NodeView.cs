using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Nodes
{
    public class NodeView : MonoBehaviour
    {
        [SerializeField]
        private List<Neighbour> neighbours = new();

        public virtual NodeType NodeType { get; } = NodeType.SimpleNode;
        
        public List<Neighbour> Neighbours => neighbours;
        
        public int ID
        {
            get
            {
                if (_id == -1)
                    _id = Random.Range(int.MinValue, int.MaxValue);

                return _id;
            }
        }
        private int _id = -1;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up, gameObject.name);
            
            if (neighbours == null) return;

            Gizmos.color = Color.cyan;

            foreach (var neighbour in neighbours)
            {
                if (neighbour?.Node == null) continue;

                Gizmos.DrawLine(transform.position, neighbour.Node.transform.position);
                
                Vector3 mid = (transform.position + neighbour.Node.transform.position) * 0.5f;
                UnityEditor.Handles.Label(mid, neighbour.Distance.ToString("F1"));
            }
        }
#endif
    }

    [Serializable]
    public class Neighbour
    {
        public NodeView Node;
        public float Distance = 1f;
    }
}