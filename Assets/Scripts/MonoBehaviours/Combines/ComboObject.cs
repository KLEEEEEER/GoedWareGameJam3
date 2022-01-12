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
                    comboObject.Touch();
                    Touch();

                    Debug.Log($"Creating comboObject of type {(Type)(_currentComboType + 1)} at {transform.position}");

                    comboObject.transform.DOScale(0, 0.2f).OnComplete(() => {
                        Destroy(comboObject.gameObject);
                    });

                    transform.DOScale(0, 0.2f).OnComplete(() => {
                        ComboObjectFactory.Instance.Create((Type)(_currentComboType + 1), transform.position);
                        Destroy(gameObject);
                    });
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
            transform.position = _instantiatedPosition;
        }

        public void SetKinematic()
        {
            _rigidbody.isKinematic = true;
        }

        public void PreventFalling()
        {
            ReturnToInitPosition();
        }
    }
}