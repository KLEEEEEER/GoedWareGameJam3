using GoedWareGameJam3.MonoBehaviours.Combines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class ExitTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject _exitForceField;
        [SerializeField] private Transform[] _animationPoints;

        [SerializeField] private GameObject _victoryFloor;
        [SerializeField] private GameObject _victoryTrigger;

        [SerializeField] private AudioSource _placingCubeSound;

        private int _animationPointsCount = 0;

        private void Start()
        {
            _animationPointsCount = _animationPoints.Length;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out ComboObject comboObject)) return;
            if (comboObject.ComboObjectType != ComboObject.Type.Type3) return;

            StartAnimation(comboObject);
        }

        private void StartAnimation(ComboObject comboObject)
        {
            _placingCubeSound.Play();

            comboObject.Draggable.Unhold();
            comboObject.Draggable.DisableDragging();
            comboObject.SetKinematic(); 
            _victoryFloor.gameObject.SetActive(true);
            _victoryTrigger.gameObject.SetActive(true);

            Sequence animationSequence = DOTween.Sequence();
            for (int i = 0; i < _animationPoints.Length; i++)
            {
                animationSequence.Append(comboObject.transform.DOMove(_animationPoints[i].position, 0.5f));
                animationSequence.Join(comboObject.transform.DORotateQuaternion(_animationPoints[i].rotation, 0.5f));
            }
            animationSequence.AppendCallback(() => { _exitForceField.gameObject.SetActive(false); });
        }
    }
}