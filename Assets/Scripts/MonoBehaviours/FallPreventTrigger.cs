using GoedWareGameJam3.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class FallPreventTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out IFallPreventable fallPreventable)) return;

            Debug.Log($"Prevent falling on {other.gameObject.name}");
            fallPreventable.PreventFalling();
        }
    }
}