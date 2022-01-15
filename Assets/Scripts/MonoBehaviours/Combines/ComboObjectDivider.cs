using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    public class ComboObjectDivider : MonoBehaviour
    {
        [SerializeField] private Transform _comboObjectPosition1;
        [SerializeField] private Transform _comboObjectPosition2;

        [SerializeField] private AudioSource _comboSound;

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out ComboObject comboObject)) return;
            if (comboObject.ComboObjectType == ComboObject.Type.Type1) return;

            switch (comboObject.ComboObjectType)
            {
                case ComboObject.Type.Type2:
                    Divide(comboObject, ComboObject.Type.Type1);
                    break;
                case ComboObject.Type.Type3:
                    Divide(comboObject, ComboObject.Type.Type2);
                    break;
            }
        }

        private void Divide(ComboObject comboObject, ComboObject.Type type)
        {
            _comboSound.Play();

            Sequence divide = DOTween.Sequence();

            divide.Append(comboObject.GetEvaporateTween()).OnComplete(() => 
            {
                ComboObjectFactory.Instance.Create(type, _comboObjectPosition1.position);
                ComboObjectFactory.Instance.Create(type, _comboObjectPosition2.position);
            });
        }
    }
}