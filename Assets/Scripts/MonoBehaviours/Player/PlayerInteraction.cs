using GoedWareGameJam3.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [RequireComponent(typeof(PlayerInputs))]
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float _dragRadius = 2f;
        [SerializeField] private LayerMask _draggableMask;
        [SerializeField] private LayerMask _interactableMask;
        [SerializeField] private SpriteRenderer _interactionButtonSprite;

        private PlayerInputs _playerInputs;

        private Draggable _currentDraggable;
        public Draggable CurrentDraggable => _currentDraggable;

        private IInteractable _currentInteractable;

        private void Awake()
        {
            _playerInputs = GetComponent<PlayerInputs>();
        }

        private void OnEnable()
        {
            _playerInputs.OnHoldPressed += OnHoldPressed;
            _playerInputs.OnHoldReleased += OnHoldReleased;
            _playerInputs.OnInteractPressed += OnInteractPressed;
        }

        private void OnDisable()
        {
            _playerInputs.OnHoldPressed -= OnHoldPressed;
            _playerInputs.OnHoldReleased -= OnHoldReleased;
            _playerInputs.OnInteractPressed -= OnInteractPressed;
        }

        private void OnHoldPressed()
        {
            Draggable closestDraggable = null;
            float closestDraggableDistance = float.MaxValue;

            Collider[] colliders = Physics.OverlapSphere(transform.position, _dragRadius, _draggableMask);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out Draggable draggable))
                {
                    if (!draggable.CanBeDragged) continue;

                    //Debug.Log("There is Draggable in hold radius");
                    //float distance = Vector3.Distance(transform.position, draggable.transform.position);
                    float distance = (draggable.transform.position - transform.position).sqrMagnitude;
                    if (distance < closestDraggableDistance)
                    {
                        closestDraggableDistance = distance;
                        closestDraggable = draggable;
                    }
                }
            }

            if (closestDraggable != null)
            {
                Hold(closestDraggable);
            }
        }

        private void OnInteractPressed()
        {
            if (_currentInteractable == null) return;

            _currentInteractable.Interact();
        }

        private void FixedUpdate()
        {
            FindInteractable();
        }

        private void FindInteractable()
        {
            IInteractable closestInteractable = null;
            float closestInteractableDistance = float.MaxValue;

            Collider[] colliders = Physics.OverlapSphere(transform.position, _dragRadius, _interactableMask);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    float distance = (collider.transform.position - transform.position).sqrMagnitude;
                    if (distance < closestInteractableDistance)
                    {
                        closestInteractableDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != null)
            {
                _currentInteractable = closestInteractable;

                if (!_interactionButtonSprite.gameObject.activeInHierarchy)
                {
                    _interactionButtonSprite.gameObject.SetActive(true);
                }
            }
            else
            {
                if (_interactionButtonSprite.gameObject.activeInHierarchy)
                {
                    _interactionButtonSprite.gameObject.SetActive(false);
                }
                _currentInteractable = null;
            }
        }

        private void Update()
        {
            if (_currentDraggable == null) 
            { 
                return; 
            }

            if (Vector3.Distance(_currentDraggable.transform.position, transform.position) > _currentDraggable.UnholdDistance)
            {
                Unhold();
            }
        }

        private void OnHoldReleased()
        {
            if (_currentDraggable == null) return;

            Unhold();
        }

        private void Hold(Draggable draggable)
        {
            Debug.Log($"Hold on {draggable.gameObject.name}");
            _currentDraggable = draggable;
            _currentDraggable.ResetVelocity();
            _currentDraggable.Hold();
            _currentDraggable.OnUnhold += Unhold;
            //_currentDraggable.transform.SetParent(transform);
        }

        private void Unhold()
        {
            Debug.Log("Unhold");
            //_currentDraggable.transform.SetParent(null);
            _currentDraggable.OnUnhold -= Unhold;
            _currentDraggable = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _dragRadius);
        }
    }
}