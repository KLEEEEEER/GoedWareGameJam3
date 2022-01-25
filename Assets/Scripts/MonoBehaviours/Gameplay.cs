using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoedWareGameJam3
{
    public class Gameplay : MonoBehaviour
    {
        private GameState _gameState;
        public GameState GameState => _gameState;

        private static Gameplay _instance;
        public static Gameplay Instance => _instance;


        private void Awake()
        {
            _instance = this;

            _gameState = new GameState();
            _gameState.TransitionTo(GameState.State.BeforePlay);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void RestartLevel()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}