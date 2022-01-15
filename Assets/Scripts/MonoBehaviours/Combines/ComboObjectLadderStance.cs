using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GoedWareGameJam3.Core.Interfaces;
using GoedWareGameJam3.MonoBehaviours.Player;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    public class ComboObjectLadderStance : MonoBehaviour, IJumpable
    {
        [SerializeField] private Transform[] _jumpPoints;

        [SerializeField] private Transform _ladderPosition;
        [SerializeField] private AudioSource _placeSound;
        private ComboObject _comboObject;

        public bool CanBeJumped => _comboObject != null;

        private void OnTriggerEnter(Collider other)
        {
            if (_comboObject != null) return;

            if (!other.gameObject.TryGetComponent(out ComboObject comboObject)) return;
            if (comboObject.ComboObjectType != ComboObject.Type.Type2) return;

            PlaceObjectToLadderPosition(comboObject);
        }

        private void PlaceObjectToLadderPosition(ComboObject comboObject)
        {
            _comboObject = comboObject;
            _comboObject.Draggable.Unhold();

            _comboObject.SetKinematic();
            Sequence movement = DOTween.Sequence();
            movement.Append(_comboObject.transform.DOMove(_ladderPosition.position, 0.5f));
            movement.Join(_comboObject.transform.DORotateQuaternion(_ladderPosition.rotation, 0.5f));
            movement.AppendCallback(() => { _comboObject.SetNotKinematic(); });

            _comboObject.Draggable.OnHold += RemoveComboObject;
            _comboObject.Draggable.OnUnhold += RemoveComboObject;

            _placeSound.Play();
        }

        private void RemoveComboObject()
        {
            _comboObject.Draggable.OnHold -= RemoveComboObject;
            _comboObject.Draggable.OnUnhold -= RemoveComboObject;
            _comboObject = null;
        }

        public void Jump(PlayerJumpInteraction playerJumpInteraction)
        {
            Debug.Log("Trying to jump!");
            playerJumpInteraction.PlayJumpAnimation(_jumpPoints);
        }
    }
}