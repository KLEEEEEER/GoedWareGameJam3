using GoedWareGameJam3.MonoBehaviours.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class PlayerFSM : FSMBase
    {
        public enum States
        {
            Disabled,
            Running,
            Jumping,
            Climbing
        }

        private void Awake()
        {
            AddState(States.Disabled, new DisabledState(this));
            AddState(States.Running, new RunningState(this));
            AddState(States.Jumping, new JumpingState(this));
            AddState(States.Climbing, new ClimbingState(this));
        }

        private void Start()
        {
            TransitionToState(States.Running);
        }
    }
}