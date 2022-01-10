using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3
{
    public class Gameplay : MonoBehaviour
    {
        private GameState _gameState;
        public GameState GameState => _gameState;

        private void Awake()
        {
            _gameState = new GameState();
            _gameState.TransitionTo(GameState.State.BeforePlay);
        }
    }
}