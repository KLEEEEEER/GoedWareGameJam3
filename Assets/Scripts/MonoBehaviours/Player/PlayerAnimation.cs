using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private float _direction = 1f;
        private float _speed = 0f;

        public void SetSpeed(float speed)
        {
            _animator.SetFloat("Speed", speed);
        }

        public void SetHorizontalDirection(float direction)
        {
            _animator.SetFloat("Direction", Mathf.Round(direction));
        }

        public void SetVerticalDirection(float direction)
        {
            if (direction == 0f)
            {
                _animator.SetBool("IsVertical", false);
            }
            else
            {
                _animator.SetBool("IsVertical", true);
            }

            _animator.SetFloat("VerticalDirection", Mathf.Round(direction));
        }

        public void Jump()
        {
            ResetTriggers();
            _animator.SetTrigger("Jump");
        }

        public void Climb()
        {
            ResetTriggers();
            _animator.SetTrigger("ClimbTallObstacle");
        }

        public void Hang()
        {
            ResetTriggers();
            _animator.SetTrigger("Hang");
        }

        private void ResetTriggers()
        {
            _animator.ResetTrigger("Jump");
            _animator.ResetTrigger("ClimbTallObstacle");
            _animator.ResetTrigger("Hang");
        }

        public void SetGrounded(bool isGrounded)
        {
            _animator.SetBool("IsGrounded", isGrounded);
        }
    }
}