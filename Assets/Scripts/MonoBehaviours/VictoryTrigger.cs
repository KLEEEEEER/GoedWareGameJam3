using GoedWareGameJam3.MonoBehaviours.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class VictoryTrigger : MonoBehaviour
    {
        [SerializeField] private int _nextLevelSceneIndex= 0;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out PlayerInteraction playerInteraction)) return;

            Gameplay.Instance.LoadScene(_nextLevelSceneIndex);
        }
    }
}