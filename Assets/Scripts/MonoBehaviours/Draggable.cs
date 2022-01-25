using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours
{
    [RequireComponent(typeof(Rigidbody))]
    public class Draggable : MonoBehaviour
    {
        public Action OnHold;
        public Action OnUnhold;

        private bool _canBeDragged = true;
        public bool CanBeDragged => _canBeDragged;

        [SerializeField] private float _unholdDistance = 2f;
        public float UnholdDistance => _unholdDistance;

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Hold()
        {
            OnHold?.Invoke();
        }

        public void Unhold()
        {
            OnUnhold?.Invoke();
        }

        public void EnableDragging()
        {
            _canBeDragged = true;
        }

        public void DisableDragging()
        {
            _canBeDragged = false;
        }

        public void Move(Vector3 movement)
        {
            Debug.Log($"Move on {gameObject.name}");
            _rigidbody.AddForce(movement * 11f, ForceMode.VelocityChange);
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, 8f);
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rigidbody.velocity = velocity;
        }

        public void ResetVelocity()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _unholdDistance);
        }
    }
}