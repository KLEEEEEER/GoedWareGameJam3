using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [RequireComponent(typeof(PlayerInputs))]
    public class PlayerDragging : MonoBehaviour
    {
        private PlayerInputs _playerInputs;

        private Draggable _currentDraggable;

        private void Awake()
        {
            _playerInputs = GetComponent<PlayerInputs>();
        }

        private void OnEnable()
        {
            _playerInputs.OnHoldPressed += OnHoldPressed;
            _playerInputs.OnHoldReleased += OnHoldReleased;
        }

        private void OnHoldPressed()
        {
            Draggable closestDraggable = null;
            float closestDistance = float.MaxValue;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<Draggable>(out Draggable draggable))
                {
                    float distance = Vector3.Distance(transform.position, draggable.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestDraggable = draggable;
                    }
                }
            }

            if (closestDraggable != null)
            {
                _currentDraggable = closestDraggable;
                _currentDraggable.transform.SetParent(transform);
            }
        }

        private void OnHoldReleased()
        {
            if (_currentDraggable == null) return;

            _currentDraggable.transform.SetParent(null);
            _currentDraggable = null;
        }
    }
}