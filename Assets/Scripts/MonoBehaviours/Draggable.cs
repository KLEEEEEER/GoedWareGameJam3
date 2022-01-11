using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours
{
    [RequireComponent(typeof(Rigidbody))]
    public class Draggable : MonoBehaviour
    {
        public Action OnUnhold;

        [SerializeField] private float _unholdDistance = 2f;
        public float UnholdDistance => _unholdDistance;

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Unhold()
        {
            OnUnhold?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _unholdDistance);
        }
    }
}