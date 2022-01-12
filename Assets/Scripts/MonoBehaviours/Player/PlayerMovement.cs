using GoedWareGameJam3.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [RequireComponent(typeof(PlayerInputs))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, IFallPreventable
    {
        [SerializeField] private PlayerSettingsSO _settings;

        private PlayerInputs _playerInputs;
        private CharacterController _characterController;

        private Vector3 _startPosition;

        private void Awake()
        {
            _playerInputs = GetComponent<PlayerInputs>();
            _characterController = GetComponent<CharacterController>();
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
            _characterController.Move(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(movementDirection.x, Physics.gravity.y, movementDirection.y) * _settings.Speed * Time.deltaTime);
        }

        public void PreventFalling()
        {
            _characterController.enabled = false;
            transform.position = _startPosition;
            _characterController.enabled = true;
        }
    }
}