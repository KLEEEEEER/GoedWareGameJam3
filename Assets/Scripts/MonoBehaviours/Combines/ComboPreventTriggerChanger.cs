using GoedWareGameJam3.Core.Interfaces;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    public class ComboPreventTriggerChanger : MonoBehaviour, IInteractable
    {
        [SerializeField] private ComboPreventTrigger _trigger;

        public void Interact()
        {
            _trigger.ChangeType();
        }
    }
}