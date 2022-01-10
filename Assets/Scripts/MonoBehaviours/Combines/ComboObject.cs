using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    public class ComboObject : MonoBehaviour
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

        private int _currentComboType;
        private int _maxTypesAmount;

        private bool _isTouchedOtherComboObject = false;
        private bool IsTouchedOtherComboObject => _isTouchedOtherComboObject;

        private void Awake()
        {
            _maxTypesAmount = Enum.GetValues(typeof(Type)).Length;
            _currentComboType = (int)_type;
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
    }
}