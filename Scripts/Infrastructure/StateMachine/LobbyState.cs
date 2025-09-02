using AtomicApps.Infrastructure.StateMachine;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public class LobbyState : IState
    {
        private GameStateMachine _stateMachine;

        public LobbyState(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}