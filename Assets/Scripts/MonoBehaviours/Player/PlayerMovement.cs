using GoedWareGameJam3.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [RequireComponent(typeof(PlayerInputs))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerInteraction))]
    public class PlayerMovement : MonoBehaviour, IFallPreventable
    {
        [SerializeField] private PlayerSettingsSO _settings;

        private PlayerInputs _playerInputs;
        private PlayerAnimation _playerAnimation;
        private PlayerInteraction _playerInteraction;
        private CharacterController _characterController;

        private Vector3 _startPosition;

        private void Awake()
        {
            _playerInputs = GetComponent<PlayerInputs>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _characterController = GetComponent<CharacterController>();
            _playerInteraction = GetComponent<PlayerInteraction>();
        }

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            Move(_playerInputs.GetInput());
        }

        private void Move(Vector2 movementDirection)
        {
            Vector3 movement = new Vector3(movementDirection.x, 0f, movementDirection.y) * _settings.Speed * Time.deltaTime;
            Vector3 movementWithGravity = movement + Physics.gravity;
            _characterController.Move(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movementWithGravity);
            if (movementDirection.normalized.x != 0f)
            {
                _playerAnimation.SetDirection(movementDirection.normalized.x);
            }
            _playerAnimation.SetSpeed(movement.normalized.sqrMagnitude);

            if (_playerInteraction.CurrentDraggable != null)
            {
                _playerInteraction.CurrentDraggable.Move(movement);
            }
        }

        public void PreventFalling()
        {
            _characterController.enabled = false;
            transform.position = _startPosition;
            _characterController.enabled = true;
        }

        public void Activate()
        {
            _characterController.enabled = true;
        }
        public void Deactivate()
        {
            _characterController.enabled = false;
        }
    }
}