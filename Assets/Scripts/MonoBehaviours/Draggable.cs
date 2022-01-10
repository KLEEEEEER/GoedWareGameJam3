using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours
{
    [RequireComponent(typeof(Rigidbody))]
    public class Draggable : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }
}