using GoedWareGameJam3.MonoBehaviours.Systems;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class OnLedgeHangingState : BaseState
    {
        private PlayerFSM _playerFSM;
        private PlayerInputs _playerInputs;
        private PlayerLedgeChecker _playerLedgeChecker;
        private PlayerAnimation _playerAnimation;
        public OnLedgeHangingState(FSMBase fsm) : base(fsm) 
        {
            _playerFSM = fsm.GetComponent<PlayerFSM>();
            _playerInputs = fsm.GetComponent<PlayerInputs>();
            _playerLedgeChecker = fsm.GetComponent<PlayerLedgeChecker>();
            _playerAnimation = fsm.GetComponent<PlayerAnimation>();
        }

        public override void EnterState()
        {
            _playerInputs.OnJumpPressed += Climb;
            _playerAnimation.Hang();
        }

        public override void ExitState()
        {
            _playerInputs.OnJumpPressed -= Climb;
        }

        private void Climb()
        {
            _playerLedgeChecker.Climb();
            _playerFSM.TransitionToState(PlayerFSM.States.Climbing);
        }
    }
}