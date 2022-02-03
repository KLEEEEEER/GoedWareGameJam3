using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ComboPreventTriggerMeshGenerator : MonoBehaviour
    {
        [HideInInspector] [SerializeField] private float _height = 4f;
        [HideInInspector] [SerializeField] private float _radius = 0.4f;
        [HideInInspector] [SerializeField] private float _standHeight = 0.5f;
        [HideInInspector] [SerializeField] private float _standWidth = 0.7f;

        [SerializeField] private GameObject _pointsHolder;
        [SerializeField] private Material _forceFieldMaterial;
        [SerializeField] private MeshFilter _standMeshFilter;
        [SerializeField] private MeshRenderer _standMeshRenderer;
        [SerializeField] private Material _standMeshMaterial;

        public Transform[] GetPoints()
        {
            if (_pointsHolder == null) return new Transform[0];

            List<Transform> childObjectsList = new List<Transform>(_pointsHolder.GetComponentsInChildren<Transform>());
            childObjectsList.Remove(_pointsHolder.transform);
            return childObjectsList.ToArray();
        }

        public void SetNewMesh(Mesh mesh)
        {
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().material = _forceFieldMaterial;
        }

        public void SetNewStandMesh(Mesh mesh)
        {
            _standMeshFilter.mesh = mesh;
            _standMeshRenderer.material = _standMeshMaterial;
        }
    }
}