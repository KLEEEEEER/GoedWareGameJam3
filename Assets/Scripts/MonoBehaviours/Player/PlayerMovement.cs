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
        [SerializeField] private AudioClip[] _footstepsSounds;
        [SerializeField] private AudioSource _footstepsSource;
        [SerializeField] private float _timeBetweenFootsteps = 0.3f;

        [SerializeField] private float _groundCheckLength = 1f;
        [SerializeField] private LayerMask _groundLayerMask;

        private PlayerInputs _playerInputs;
        private PlayerAnimation _playerAnimation;
        private PlayerInteraction _playerInteraction;
        private CharacterController _characterController;

        private Vector3 _startPosition;
        private Vector3 _velocity = Vector3.zero;
        private bool _isGrounded = false;
        private bool _isJumping = false;

        private float _timeFromPreviousFootstepPlayed = 0f;

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

        private void FixedUpdate()
        {
            _isGrounded = Physics.Raycast(transform.position, -transform.up, _groundCheckLength, _groundLayerMask);
        }

        private void Move(Vector2 movementDirection)
        {
            PlayFootstepSounds(movementDirection);

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

            /*if (_characterController.isGrounded)
            {
                _velocity.y = 0f;
            }
            else
            {
                _velocity += Physics.gravity;
            }*/

            if (_isGrounded)
            {
                //_velocity.y = 0f;
                _isJumping = false;
            }
            else
            {
                _velocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            }

            if (_playerInputs.IsJumpPressed && _isGrounded && !_isJumping)
            {
                Debug.Log("Jumping!");
                //_velocity.y += Mathf.Sqrt(_settings.JumpHeight * -3.0f * Physics.gravity.y);
                _velocity.y = _settings.JumpHeight * _settings.Speed;
                //_velocity.y = Mathf.Sqrt(2f * _settings.JumpHeight * -Physics.gravity.y); ;
                _isJumping = true;
            }

            Debug.Log($"_velocity = {_velocity}");

            _characterController.Move(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * _velocity * Time.deltaTime);

            if (movementDirection.normalized.x != 0f)
            {
                _playerAnimation.SetHorizontalDirection(movementDirection.normalized.x);
            }
            _playerAnimation.SetVerticalDirection(movementDirection.normalized.y);
        }

        private void RotateModel(Vector3 direction)
        {
            Vector3 newRotation = Vector3.RotateTowards(_model.forward, direction, _settings.RotationSpeed * Time.deltaTime, 0.0f);
            _model.rotation = Quaternion.LookRotation(newRotation);
        }

        private void PlayFootstepSounds(Vector2 movementDirection)
        {
            if (movementDirection.normalized.sqrMagnitude > 0f && _timeFromPreviousFootstepPlayed >= _timeBetweenFootsteps)
            {
                AudioClip randomFootstep = _footstepsSounds[Random.Range(0, _footstepsSounds.Length)];
                _footstepsSource.clip = randomFootstep;
                _footstepsSource.pitch = Random.Range(0.8f, 1.2f);
                _footstepsSource.Play();
                _timeFromPreviousFootstepPlayed = 0f;
            }
            else
            {
                _timeFromPreviousFootstepPlayed += Time.deltaTime;
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * _groundCheckLength);
        }
    }
}