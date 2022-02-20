﻿using GoedWareGameJam3.MonoBehaviours.Systems;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class RunningState : BaseState
    {
        private PlayerFSM _playerFSM;
        private PlayerMovement _playerMovement;
        private PlayerInputs _playerInputs;
        private PlayerLedgeChecker _playerLedgeChecker;
        public RunningState(FSMBase fsm) : base(fsm) 
        {
            _playerFSM = fsm.GetComponent<PlayerFSM>();
            _playerMovement = fsm.GetComponent<PlayerMovement>();
            _playerInputs = fsm.GetComponent<PlayerInputs>();
            _playerLedgeChecker = fsm.GetComponent<PlayerLedgeChecker>();
        }

        public override void EnterState()
        {
            _playerMovement.Activate();
        }

        public override void FixedUpdate()
        {
            _playerLedgeChecker.Check();
        }

        public override void Update()
        {
            _playerMovement.Move(_playerInputs.GetInput());
        }
    }
}