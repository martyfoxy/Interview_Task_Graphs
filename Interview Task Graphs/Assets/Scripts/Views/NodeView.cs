using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Views
{
    public class NodeView : MonoBehaviour
    {
        [SerializeField]
        private List<Neighbour> neighbours = new();
        
        public bool HasNeighbour(NodeView other)
        {
            return neighbours.Exists(n => n.Node == other);
        }

        public void AddNeighbour(NodeView other, float distance)
        {
            neighbours.Add(new Neighbour { Node = other, Distance = distance });
        }

        public void SetNeighbourDistance(NodeView other, float distance)
        {
            neighbours.FirstOrDefault(x => x.Node == other)!.Distance = distance;
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            foreach (var neighbour in neighbours)
            {
                if (neighbour.Node == null || neighbour.Node == this)
                    continue;

                if (neighbour.Node.HasNeighbour(this))
                    neighbour.Node.SetNeighbourDistance(this, neighbour.Distance);
                else
                    neighbour.Node.AddNeighbour(this, neighbour.Distance);
            }
        }

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