using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public class BootstrapState : IState
    {
        private GameStateMachine _stateMachine;
        private SceneLoaderService _sceneLoaderService;
        private IAudioService _audioService;

        public BootstrapState(GameStateMachine stateMachine, SceneLoaderService sceneLoaderService, IAudioService audioService)
        {
            _audioService = audioService;
            _stateMachine = stateMachine;
            _sceneLoaderService = sceneLoaderService;
        }
        
        public async void Enter()
        {
            _audioService.PlayMusic(SoundKeys.MUSIC_MENU);
            
            if (SceneManager.GetActiveScene().name != SceneName.Loader.ToString())
            {
                await _sceneLoaderService.LoadScene(SceneName.Loader, false, OnSceneLoaded);
            }
        }

        private async void OnSceneLoaded()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}