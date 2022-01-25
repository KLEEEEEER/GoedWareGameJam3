using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    [RequireComponent(typeof(MeshFilter))]
    public class ComboPreventTriggerMeshGenerator : MonoBehaviour
    {
        [SerializeField] private float _height = 4f;
        [SerializeField] private float _radius = 0.4f;

        public void SetNewMesh(Mesh mesh)
        {
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}