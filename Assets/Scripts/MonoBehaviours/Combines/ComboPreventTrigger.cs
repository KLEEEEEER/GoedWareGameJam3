using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    public class ComboPreventTrigger : MonoBehaviour
    {
        [SerializeField] private ComboObject.Type _type = ComboObject.Type.Type1;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private ComboPreventingChangerRulesSettingsSO _rulesSettings;

        public Material CurrentMaterial => _renderer.material;

        int _changeIndex = -1;

        private void Awake()
        {
            if (_rulesSettings != null)
            {
                ComboPreventingChangerRule? comboPreventingRule = _rulesSettings.Rules.Where(x => x.Type == _type).FirstOrDefault();
                if (comboPreventingRule != null)
                {
                    _changeIndex = Array.IndexOf(_rulesSettings.Rules, comboPreventingRule);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out ComboObject comboObject)) return;

            if (comboObject.ComboObjectType != _type)
            {
                comboObject.ReturnToInitPosition();
            }
        }

        public void ChangeType()
        {
            if (_rulesSettings == null) return;

            _changeIndex++;

            if (_changeIndex >= _rulesSettings.Rules.Length)
            {
                _changeIndex = 0;
            }

            _type = _rulesSettings.Rules[_changeIndex].Type;
            _renderer.material = _rulesSettings.Rules[_changeIndex].Material;
        }
    }
}