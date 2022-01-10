using System;

namespace GoedWareGameJam3 
{ 
    public class GameState
    {
        private State _currentState = State.BeforePlay;

        public Action OnBeforePlay;
        public Action OnPaused;
        public Action OnGameplay;
        public Action OnGameOver;
        public Action OnVictory;

        public enum State
        {
            BeforePlay,
            Paused,
            Gameplay,
            GameOver,
            Victory
        }

        public void TransitionTo(State state)
        {
            if (_currentState == state) return;

            switch (state)
            {
                case State.BeforePlay:
                    OnBeforePlay?.Invoke();
                    break;
                case State.Paused:
                    OnPaused?.Invoke();
                    break;
                case State.Gameplay:
                    OnGameplay?.Invoke();
                    break;
                case State.GameOver:
                    OnGameOver?.Invoke();
                    break;
                case State.Victory:
                    OnVictory?.Invoke();
                    break;
            }

            _currentState = state;
        }

        public bool CompareCurrentState(State state)
        {
            return _currentState == state;
        }
    }
}