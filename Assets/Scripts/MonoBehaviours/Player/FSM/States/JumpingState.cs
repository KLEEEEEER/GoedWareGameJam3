using GoedWareGameJam3.MonoBehaviours.Systems;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class JumpingState : BaseState
    {
        private PlayerFSM _playerFSM;
        public JumpingState(FSMBase fsm) : base(fsm) 
        {
            _playerFSM = fsm.GetComponent<PlayerFSM>();
        }

        public override void EnterState()
        {

        }

        public override void Update()
        {

        }
    }
}