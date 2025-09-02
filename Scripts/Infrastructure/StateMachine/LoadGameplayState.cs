using AtomicApps.Infrastructure.StateMachine;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public class LoadGameplayState : IPayloadedState<bool>
    {
        private GameStateMachine _stateMachine;
        private SceneLoaderService _sceneLoaderService;

        public LoadGameplayState(GameStateMachine stateMachine, SceneLoaderService sceneLoaderService)
        {
            _stateMachine = stateMachine;
            _sceneLoaderService = sceneLoaderService;
        }

        private void OnSceneLoaded()
        {
            _stateMachine.Enter<GameplayState>();
        }

        public async void Enter(bool showCurtain)
        {
            await _sceneLoaderService.LoadScene(SceneName.Gameplay, showCurtain, OnSceneLoaded);
        }

        public void Exit()
        {
            
        }
    }
}