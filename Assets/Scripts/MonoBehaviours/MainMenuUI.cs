using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _startButton;

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
            Gameplay.Instance.LoadScene(1);
        }
    }
}