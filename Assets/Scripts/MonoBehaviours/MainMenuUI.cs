using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private CanvasGroup _blackScreen;
        [SerializeField] private float _timeToFadeOut = 1f;
        [Space]
        [SerializeField] private CanvasGroup _gameNamePart1;
        [SerializeField] private CanvasGroup _gameNamePart2;
        [Space]
        [SerializeField] private CinemachineVirtualCamera _mainVirtualCamera;
        [SerializeField] private Transform _cameraEndPosition;
        [Space]
        [SerializeField] private Rigidbody[] _rigidbodies;

        private void Awake()
        {
            _blackScreen.alpha = 1f;
            _gameNamePart1.alpha = 0f;
            _gameNamePart2.alpha = 0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        private void Start()
        {
            MoveCamera();
            StartCoroutine(FadeOutBlackScreen());

            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.useGravity = true;
            }
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(LoadFirstLevel);
            _creditsButton.onClick.AddListener(LoadCreditsScene);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(LoadFirstLevel);
            _creditsButton.onClick.RemoveListener(LoadCreditsScene);
            _exitButton.onClick.RemoveListener(ExitGame);
        }

        private void LoadFirstLevel()
        {
            StartCoroutine(FadeInBlackScreen(() => 
            {
                SceneManager.LoadScene(2);
            }));
        }

        private void LoadCreditsScene()
        {
            StartCoroutine(FadeInBlackScreen(() =>
            {
                SceneManager.LoadScene(6);
            }));
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

            time = 0f;
            while (time <= 0.3f)
            {
                _gameNamePart1.alpha = Mathf.Lerp(0f, 1f, time / 0.3f);

                time += Time.deltaTime;
                yield return null;
            }
            _gameNamePart1.alpha = 1f;

            yield return new WaitForSeconds(1.3f);

            time = 0f;
            while (time <= 0.3f)
            {
                _gameNamePart2.alpha = Mathf.Lerp(0f, 1f, time / 0.3f);

                time += Time.deltaTime;
                yield return null;
            }
            _gameNamePart2.alpha = 1f;
        }

        private IEnumerator FadeInBlackScreen(Action callback)
        {
            float time = 0f;
            while (time <= _timeToFadeOut)
            {
                _blackScreen.alpha = Mathf.Lerp(0f, 1f, time / _timeToFadeOut);

                time += Time.deltaTime;
                yield return null;
            }
            callback.Invoke();
        }

        private void MoveCamera()
        {
            _mainVirtualCamera.transform.DOMove(_cameraEndPosition.position, 2f).SetEase(Ease.InOutSine);
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}