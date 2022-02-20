using GoedWareGameJam3.MonoBehaviours.Systems;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class OnLedgeHangingState : BaseState
    {
        private PlayerFSM _playerFSM;
        private PlayerInputs _playerInputs;
        private PlayerMovement _playerMovement;
        private PlayerLedgeChecker _playerLedgeChecker;
        private PlayerAnimation _playerAnimation;
        public OnLedgeHangingState(FSMBase fsm) : base(fsm) 
        {
            _playerFSM = fsm.GetComponent<PlayerFSM>();
            _playerInputs = fsm.GetComponent<PlayerInputs>();
            _playerMovement = fsm.GetComponent<PlayerMovement>();
            _playerLedgeChecker = fsm.GetComponent<PlayerLedgeChecker>();
            _playerAnimation = fsm.GetComponent<PlayerAnimation>();
        }

        public override void EnterState()
        {
            _playerMovement.SetRotationModel(-_playerLedgeChecker.LedgeNormal);

            _playerInputs.OnJumpPressed += Climb;
            _playerAnimation.Hang();
        }

        public override void ExitState()
        {
            _playerInputs.OnJumpPressed -= Climb;
        }

        public override void Update()
        {
            _playerMovement.LedgeMove(_playerInputs.GetInput(), _playerLedgeChecker.LedgeNormal);
        }

        private void Climb()
        {
            _playerLedgeChecker.Climb();
            _playerFSM.TransitionToState(PlayerFSM.States.Climbing);
        }
    }
}