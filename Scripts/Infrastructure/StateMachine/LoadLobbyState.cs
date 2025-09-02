using AtomicApps.Infrastructure.StateMachine;
using UnityEngine.SceneManagement;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public class LoadLobbyState : IPayloadedState<bool>
    {
        private GameStateMachine _stateMachine;
        private SceneLoaderService _sceneLoaderService;

        public LoadLobbyState(GameStateMachine stateMachine, SceneLoaderService sceneLoaderService)
        {
            _stateMachine = stateMachine;
            _sceneLoaderService = sceneLoaderService;
        }

        public async void Enter(bool showCurtain)
        {
            await _sceneLoaderService.LoadScene(SceneName.Lobby, showCurtain, OnSceneLoaded);
        }

        private void OnSceneLoaded()
        {
            _stateMachine.Enter<LobbyState>();
        }

        public void Exit()
        {
            
        }
    }
}