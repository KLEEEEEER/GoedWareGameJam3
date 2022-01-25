using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _blackScreen;
        private float _timeToFadeOut = 0.3f;

        private static GameplayUI _instance;
        public static GameplayUI Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            _blackScreen.alpha = 1f;
        }

        private void Start()
        {
            StartCoroutine(FadeOutBlackScreen());
        }

        public void FadeIn(float time)
        {
            StartCoroutine(FadeInBlackScreen(time));
        }

        private IEnumerator FadeOutBlackScreen()
        {
            float time = 0f;
            while (time <= _timeToFadeOut)
            {
                _blackScreen.alpha = Mathf.Lerp(1f, 0f, time / _timeToFadeOut);

                time += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator FadeInBlackScreen(float fadeTime = 1f)
        {
            float time = 0f;
            while (time <= _timeToFadeOut)
            {
                _blackScreen.alpha = Mathf.Lerp(0f, 1f, time / fadeTime);

                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}