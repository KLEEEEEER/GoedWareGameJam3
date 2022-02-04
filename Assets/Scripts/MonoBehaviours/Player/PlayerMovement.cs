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
        [SerializeField] private float _gravityMultiplier = 2f;
        [SerializeField] private Transform _model;

        [SerializeField] private float _groundCheckLength = 1f;
        [SerializeField] private LayerMask _groundLayerMask;

        private PlayerInputs _playerInputs;
        private PlayerAnimation _playerAnimation;
        private PlayerInteraction _playerInteraction;
        private CharacterController _characterController;
        private PlayerAudio _playerAudio;

        private Vector3 _startPosition;
        private Vector3 _velocity = Vector3.zero;
        private bool _isGrounded = false;
        private bool _isJumping = false;


        private void Awake()
        {
            _playerInputs = GetComponent<PlayerInputs>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _characterController = GetComponent<CharacterController>();
            _playerInteraction = GetComponent<PlayerInteraction>();
            _playerAudio = GetComponent<PlayerAudio>();
        }

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            Move(_playerInputs.GetInput());
        }

        private void FixedUpdate()
        {
            _isGrounded = Physics.Raycast(transform.position, -transform.up, _groundCheckLength, _groundLayerMask);
            _playerAnimation.SetGrounded(_isGrounded);
        }

        private void Move(Vector2 movementDirection)
        {
            Vector3 direction = new Vector3(movementDirection.x, 0f, movementDirection.y);

            RotateModel(direction);

            direction.Normalize();
            _playerAnimation.SetSpeed(direction.sqrMagnitude);

            direction *= _settings.Speed;

            _velocity.x = direction.x;
            _velocity.z = direction.z;


            if (_playerInteraction.CurrentDraggable != null)
            {
                _playerInteraction.CurrentDraggable.Move(_velocity * Time.deltaTime);
            }

            if (_isGrounded)
            {
                _isJumping = false;
            }
            else
            {
                _velocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            }

            if (_playerInputs.IsJumpPressed && _isGrounded && !_isJumping)
            {
                _velocity.y = _settings.JumpHeight * _settings.Speed;
                _isJumping = true;
                _playerAnimation.Jump();
            }

            Debug.Log($"_velocity = {_velocity}");

            _characterController.Move(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * _velocity * Time.deltaTime);

            if (movementDirection.normalized.x != 0f)
            {
                _playerAnimation.SetHorizontalDirection(movementDirection.normalized.x);
            }
            _playerAnimation.SetVerticalDirection(movementDirection.normalized.y);

            _playerAudio.PlayFootstepSounds(movementDirection);
        }

        private void RotateModel(Vector3 direction)
        {
            Vector3 newRotation = Vector3.RotateTowards(_model.forward, direction, _settings.RotationSpeed * Time.deltaTime, 0.0f);
            _model.rotation = Quaternion.LookRotation(newRotation);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * _groundCheckLength);
        }
    }
}