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

            StartCoroutine(LoadingNewSceneCoroutine());
        }

        private IEnumerator LoadingNewSceneCoroutine()
        {
            GameplayUI.Instance.FadeIn(0.3f);
            yield return new WaitForSeconds(0.4f);
            Debug.Log($"Loading scene with index {_nextLevelSceneIndex}");
            Gameplay.Instance.LoadScene(_nextLevelSceneIndex);
        }
    }
}