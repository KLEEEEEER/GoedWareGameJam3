using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Systems
{
    public class BaseState
    {
        protected FSMBase fsm;

        public BaseState(FSMBase FSM)
        {
            fsm = FSM;
        }
        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void Update() { }
        public virtual void OnTriggerEnter(Collider collision) { }
        public virtual void OnTriggerExit(Collider collision) { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}