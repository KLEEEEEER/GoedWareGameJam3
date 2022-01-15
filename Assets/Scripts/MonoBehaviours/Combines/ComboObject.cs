using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GoedWareGameJam3.Core.Interfaces;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    [RequireComponent(typeof(Draggable))]
    [RequireComponent(typeof(Rigidbody))]
    public class ComboObject : MonoBehaviour, IFallPreventable
    {
        public enum Type
        {
            Type1,
            Type2,
            Type3
        }

        [SerializeField] private Type _type = Type.Type1;
        public Type ComboObjectType => _type;

        private Vector3 _instantiatedPosition;
        private Rigidbody _rigidbody;
        private Draggable _draggable;
        public Draggable Draggable => _draggable;

        private int _currentComboType;
        private int _maxTypesAmount;

        private bool _isTouchedOtherComboObject = false;
        private bool IsTouchedOtherComboObject => _isTouchedOtherComboObject;

        private void Awake()
        {
            _maxTypesAmount = Enum.GetValues(typeof(Type)).Length;
            _currentComboType = (int)_type;

            _draggable = GetComponent<Draggable>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _instantiatedPosition = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isTouchedOtherComboObject) return;

            if (collision.gameObject.TryGetComponent(out ComboObject comboObject))
            {
                if (comboObject.ComboObjectType == ComboObjectType && _currentComboType <= (_maxTypesAmount - 1))
                {
                    ComboObjectFactory.Instance.PlayCombineSound();

                    comboObject.Draggable.Unhold();
                    Draggable.Unhold();

                    comboObject.Touch();
                    Touch();

                    Debug.Log($"Creating comboObject of type {(Type)(_currentComboType + 1)} at {transform.position}");

                    comboObject.transform.DOScale(0, 0.4f).OnComplete(() => {
                        Destroy(comboObject.gameObject);
                    });

                    transform.DOScale(0, 0.4f).OnComplete(() => {
                        ComboObjectFactory.Instance.Create((Type)(_currentComboType + 1), transform.position);
                        Destroy(gameObject);
                    });

                    comboObject.transform.DOMove(collision.contacts[0].point, 0.2f);
                    transform.DOMove(collision.contacts[0].point, 0.2f);
                }
            }
        }

        public void Touch()
        {
            _isTouchedOtherComboObject = true;
        }

        public void ReturnToInitPosition()
        {
            _draggable.Unhold();
            _draggable.ResetVelocity();
            SetKinematic();

            Sequence returningAnimation = DOTween.Sequence();
            Vector3 startScale = transform.localScale;

            returningAnimation.Append(transform.DOMoveY(transform.position.y + 3f, 0.4f));
            returningAnimation.Append(transform.DOScale(0f, 0.3f));

            returningAnimation.AppendCallback(() =>
            {
                transform.position = _instantiatedPosition + new Vector3(0f, 2f, 0f);
                transform.DOScale(startScale.x, 0f);
                SetNotKinematic();
            });
        }

        public void SetKinematic()
        {
            _rigidbody.isKinematic = true;
        }

        public void SetNotKinematic()
        {
            _rigidbody.isKinematic = false;
        }

        public void PreventFalling()
        {
            ReturnToInitPosition();
        }

        public Tween GetEvaporateTween()
        {
            _draggable.Unhold();
            _draggable.ResetVelocity();
            SetKinematic();

            return transform.DOScale(0, 0.5f).OnComplete(() => {
                Destroy(gameObject);
            });
        }
    }
}