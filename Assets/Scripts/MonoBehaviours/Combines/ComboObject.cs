using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private int _currentComboType;
        private int _maxTypesAmount;

        private bool _isTouchedOtherComboObject = false;
        private bool IsTouchedOtherComboObject => _isTouchedOtherComboObject;

        private void Awake()
        {
            _maxTypesAmount = Enum.GetValues(typeof(Type)).Length;
            _currentComboType = (int)_type;
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
                    ComboObjectFactory.Instance.Create((Type)(_currentComboType + 1), transform.position);

                    Destroy(comboObject.gameObject);
                    Destroy(gameObject);
                }
            }
        }

        public void Touch()
        {
            _isTouchedOtherComboObject = true;
        }
    }
}