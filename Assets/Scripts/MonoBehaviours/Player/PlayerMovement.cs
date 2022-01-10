using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [RequireComponent(typeof(PlayerInputs))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerSettingsSO _settings;

        private PlayerInputs _playerInputs;
        private CharacterController _characterController;

        private void Awake()
        {
            _playerInputs = GetComponent<PlayerInputs>();
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Move(_playerInputs.GetInput());
        }

        private void Move(Vector2 movementDirection)
        {
            Debug.Log($"movementDirection = {movementDirection}");
            _characterController.Move(new Vector3(movementDirection.x, Physics.gravity.y, movementDirection.y) * _settings.Speed * Time.deltaTime);
        }
    }
}