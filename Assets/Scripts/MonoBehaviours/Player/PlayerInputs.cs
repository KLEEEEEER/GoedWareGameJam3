using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class PlayerInputs : MonoBehaviour
    {
        public Action OnInteractPressed;
        public Action OnHoldPressed;
        public Action OnHoldReleased;
        public Action OnJumpPressed;
        public Action OnRestartPressed;

        public bool IsJumpPressed => _keyInputActions.Player.Jump.triggered;

        private KeyInputActions _keyInputActions;

        private void Awake()
        {
            _keyInputActions = new KeyInputActions();
            ActivateInput();
        }

        private void OnEnable()
        {
            _keyInputActions.Player.Hold.started += HoldStarted;
            _keyInputActions.Player.Hold.canceled += HoldCanceled;
            _keyInputActions.Player.Interact.started += InteractPressed;
            _keyInputActions.Player.Jump.started += JumpPressed;
            _keyInputActions.Player.Restart.started += RestartPressed;
        }

        private void OnDisable()
        {
            _keyInputActions.Player.Hold.started -= HoldStarted;
            _keyInputActions.Player.Hold.canceled -= HoldCanceled;
            _keyInputActions.Player.Interact.started -= InteractPressed;
            _keyInputActions.Player.Jump.started -= JumpPressed;
            _keyInputActions.Player.Restart.started -= RestartPressed;
        }

        private void HoldStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnHoldPressed?.Invoke();
        }

        private void HoldCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnHoldReleased?.Invoke();
        }

        private void InteractPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnInteractPressed?.Invoke();
        }

        private void JumpPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnJumpPressed?.Invoke();
        }

        private void RestartPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Gameplay.Instance.RestartLevel();
        }

        public void ActivateInput()
        {
            _keyInputActions.Enable();
        }

        public void DeactivateInput()
        {
            _keyInputActions.Disable();
        }

        public Vector2 GetInput()
        {
            return _keyInputActions.Player.Move.ReadValue<Vector2>();
        }
    }
}