using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class Credits : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private Transform _cameraEndPoint;

        [SerializeField] private CanvasGroup[] _creditScreens;

        [SerializeField] private float _fadeSpeed = 1f;

        [SerializeField] private CanvasGroup _blackScreen;
        [SerializeField] private float _timeToFadeOut = 1f;

        private void Start()
        {
            _camera.transform.DOMove(_cameraEndPoint.position, 60f);

            StartCoroutine(FadeOutBlackScreen());
            StartCoroutine(ShowCreditTexts());
        }

        private IEnumerator ShowCreditTexts()
        {
            yield return new WaitForSeconds(1f);
            foreach (CanvasGroup creditScreen in _creditScreens)
            {
                yield return FadeInScreen(creditScreen);
                yield return new WaitForSeconds(3f);
                yield return FadeOutScreen(creditScreen);
            }
        }

        private IEnumerator FadeInScreen(CanvasGroup screen)
        {
            float fadeAmount = 0f;
            screen.alpha = fadeAmount;
            yield return null;
            while (fadeAmount <= 1f)
            {
                fadeAmount += _fadeSpeed * Time.deltaTime;
                screen.alpha = fadeAmount;
                yield return null;
            }
            screen.alpha = 1f;
        }

        private IEnumerator FadeOutScreen(CanvasGroup screen)
        {
            float fadeAmount = 1f;
            screen.alpha = fadeAmount;
            yield return null;
            while (fadeAmount >= 0f)
            {
                fadeAmount -= _fadeSpeed * Time.deltaTime;
                screen.alpha = fadeAmount;
                yield return null;
            }
            screen.alpha = 0f;
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
    }
}