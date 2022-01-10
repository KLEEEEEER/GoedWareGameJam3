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

        private int _currentComboType;
        private int _maxTypesAmount;

        private void Awake()
        {
            _maxTypesAmount = Enum.GetValues(typeof(Type)).Length;
            _currentComboType = (int)_type;
            Debug.Log($"_maxTypesAmount = {_maxTypesAmount}");
        }
    }
}