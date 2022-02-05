using GoedWareGameJam3.MonoBehaviours.Systems;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class DisabledState : BaseState
    {
        private PlayerFSM _playerFSM;
        public DisabledState(FSMBase fsm) : base(fsm)
        {
            _playerFSM = fsm.GetComponent<PlayerFSM>();
        }

        public override void EnterState()
        {

        }
    }
}