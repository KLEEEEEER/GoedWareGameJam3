using GoedWareGameJam3.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerJumpInteraction : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private PlayerInputs _playerInputs;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerInputs = GetComponent<PlayerInputs>();
        }

        private void OnEnable()
        {
            _playerInputs.OnJumpPressed += OnJumpPressed;
        }

        private void OnDisable()
        {
            _playerInputs.OnJumpPressed -= OnJumpPressed;
        }

        private void OnJumpPressed()
        {
            IJumpable closestJumpable = null;
            float closestJumpableDistance = float.MaxValue;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out IJumpable jumpable))
                {
                    if (!jumpable.CanBeJumped) continue;

                    //Debug.Log("There is Draggable in hold radius");
                    //float distance = Vector3.Distance(transform.position, draggable.transform.position);
                    float distance = (collider.transform.position - transform.position).sqrMagnitude;
                    if (distance < closestJumpableDistance)
                    {
                        closestJumpableDistance = distance;
                        closestJumpable = jumpable;
                    }
                }
            }

            if (closestJumpable != null)
            {
                closestJumpable.Jump(this);
            }
        }

        public void PlayJumpAnimation(Transform[] jumpPoints)
        {
            _playerInputs.DeactivateInput();
            _playerMovement.Deactivate();

            Sequence jump = DOTween.Sequence();
            foreach (Transform jumpPoint in jumpPoints)
            {
                jump.Append(transform.DOMove(jumpPoint.position, 0.3f));
            }
            jump.AppendCallback(() =>
            {
                _playerInputs.ActivateInput();
                _playerMovement.Activate();
            });
        }
    }
}