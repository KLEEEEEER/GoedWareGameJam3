using GoedWareGameJam3.MonoBehaviours.Combines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Tutorials
{
    public class HoldCubeTutorial : MonoBehaviour
    {
        [SerializeField] private ComboObject _comboObject;
        [SerializeField] private CanvasGroup _imageAlpha;
        [SerializeField] private float _timeToAppear = 2f;

        private void Awake()
        {
            _imageAlpha.alpha = 0f;
        }

        private void Start()
        {
            _comboObject.Draggable.OnHold += Disable;

            StartCoroutine(AnimateAlphaIn());
        }
        private void OnDisable()
        {
            _comboObject.Draggable.OnHold -= Disable;
        }

        private IEnumerator AnimateAlphaIn()
        {
            yield return new WaitForSeconds(2f);

            float currentTime = 0f;
            while (currentTime <= _timeToAppear)
            {
                _imageAlpha.alpha = Mathf.Lerp(0f, 1f, currentTime / _timeToAppear);

                currentTime += Time.deltaTime;
                yield return null;
            }
            _imageAlpha.alpha = 1f;
        }

        private IEnumerator AnimateAlphaOut()
        {
            float currentTime = 0f;
            while (currentTime <= _timeToAppear)
            {
                _imageAlpha.alpha = Mathf.Lerp(1f, 0f, currentTime / _timeToAppear);

                currentTime += Time.deltaTime;
                yield return null;
            }
            _imageAlpha.alpha = 0f;
            gameObject.SetActive(false);
        }

        private void Disable()
        {
            StartCoroutine(AnimateAlphaOut());
        }
    }
}