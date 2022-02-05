using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Systems
{
    public class FSMBase : MonoBehaviour
    {
        private readonly Dictionary<Enum, BaseState> _states = new Dictionary<Enum, BaseState>();
        private readonly Dictionary<Type, Component> _cachedComponents = new Dictionary<Type, Component>();

        [SerializeField] private FSMDebugging debugging;
        public BaseState CurrentState { get; protected set; }

        public void AddState(Enum index, BaseState state)
        {
            _states.Add(index, state);
        }

        public void RemoveState(Enum index)
        {
            _states.Remove(index);
        }

        public void TransitionToState(Enum index)
        {
            if (_states.TryGetValue(index, out var value))
            {
                if (CurrentState != null)
                {
                    CurrentState.ExitState();
                }
                CurrentState = value;
                CurrentState.EnterState();

                debugging.SetCurrentState(CurrentState.ToString());
            }
            else
            {
                Debug.LogError($"No state with index \"{index}\" in fsm.");
            }
        }

        public bool CompareState(BaseState state)
        {
            return CurrentState == state;
        }

        public bool CompareState(Enum index)
        {
            if (_states.TryGetValue(index, out var value))
            {
                return CurrentState == value;
            }
            else
            {
                Debug.LogError($"No state with index \"{index}\" in fsm.");
                return false;
            }
        }

        public new bool TryGetComponent<T>(out T component) where T : Component
        {
            var type = typeof(T);
            if (!_cachedComponents.TryGetValue(type, out var value))
            {
                if (base.TryGetComponent<T>(out component))
                    _cachedComponents.Add(type, component);

                return component != null;
            }

            component = (T)value;
            return true;
        }

        public new T GetComponent<T>() where T : Component
        {
            var type = typeof(T);
            T component = null;
            if (!_cachedComponents.TryGetValue(type, out var value))
            {
                if (base.TryGetComponent<T>(out component))
                    _cachedComponents.Add(type, component);

                return component;
            }

            component = (T)value;
            return component;
        }

        private void Update()
        {
            if (CurrentState != null)
                CurrentState.Update();
        }
        private void FixedUpdate()
        {
            if (CurrentState != null)
                CurrentState.FixedUpdate();
        }

        private void LateUpdate()
        {
            if (CurrentState != null)
                CurrentState.LateUpdate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CurrentState != null)
                CurrentState.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (CurrentState != null)
                CurrentState.OnTriggerExit(other);
        }

        public void CallDelayedOperation(float time, Action callback)
        {
            StartCoroutine(delayedOperationCoroutine(time, callback));
        }

        private IEnumerator delayedOperationCoroutine(float time, Action callback)
        {
            yield return new WaitForSeconds(time);

            callback?.Invoke();
        }
    }

    [Serializable]
    public class FSMDebugging
    {
        [SerializeField] private string currentState = "";
        public void SetCurrentState(string stateName)
        {
            currentState = stateName;
        }
    }
}