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
        [SerializeField] private SpriteRenderer _jumpButtonSprite;

        private PlayerMovement _playerMovement;
        private PlayerInputs _playerInputs;

        private IJumpable _currentJumpable;

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

        private void FixedUpdate()
        {
            IJumpable closestJumpable = null;
            float closestJumpableDistance = float.MaxValue;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 4f);
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
                _currentJumpable = closestJumpable;

                if (!_jumpButtonSprite.gameObject.activeInHierarchy)
                {
                    _jumpButtonSprite.gameObject.SetActive(true);
                }
            }
            else
            {
                _currentJumpable = null;

                if (_jumpButtonSprite.gameObject.activeInHierarchy)
                {
                    _jumpButtonSprite.gameObject.SetActive(false);
                }
            }
        }

        private void OnJumpPressed()
        {
            if (_currentJumpable != null)
            {
                _currentJumpable.Jump(this);
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