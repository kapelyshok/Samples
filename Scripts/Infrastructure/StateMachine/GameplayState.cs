using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.StateMachine;

namespace AtomicApps.Infrastructure.Bootstrap
{
    internal class GameplayState : IState
    {
        private GameStateMachine _stateMachine;
        private IAudioService _audioService;

        public GameplayState(GameStateMachine stateMachine, IAudioService audioService)
        {
            _audioService = audioService;
            _stateMachine = stateMachine;
        }
        
        public void Enter()
        {
            _audioService.PlayMusic(SoundKeys.MUSIC_STAGES);
        }

        public void Exit()
        {
            
        }
    }
}