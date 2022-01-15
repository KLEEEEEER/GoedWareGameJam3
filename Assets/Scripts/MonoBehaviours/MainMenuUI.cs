using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private CanvasGroup _blackScreen;
        [SerializeField] private float _timeToFadeOut = 1f;

        private void Start()
        {
            StartCoroutine(FadeOutBlackScreen());
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(LoadFirstLevel);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(LoadFirstLevel);
        }

        private void LoadFirstLevel()
        {
            StartCoroutine(FadeInBlackScreen());
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

        private IEnumerator FadeInBlackScreen()
        {
            float time = 0f;
            while (time <= _timeToFadeOut)
            {
                _blackScreen.alpha = Mathf.Lerp(0f, 1f, time / _timeToFadeOut);

                time += Time.deltaTime;
                yield return null;
            }
            Gameplay.Instance.LoadScene(1);
        }
    }
}