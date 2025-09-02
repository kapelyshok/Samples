using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Scpts.Mechanics.Lobby.Hearts;
using AtomicApps.UI.Mechanics;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class LevelFailedPopup : BasePopup
    {
        [SerializeField]
        private CustomButton restartButton;
        [SerializeField]
        private CustomButton homeButton;
        [SerializeField]
        private CustomButton closeButton;
        
        private GameStateMachine _stateMachine;
        private IAudioService _audioService;
        private IHearthService _hearthService;

        [Inject]
        private void Construct(GameStateMachine stateMachine, IAudioService audioService, IHearthService hearthService)
        {
            _hearthService = hearthService;
            _audioService = audioService;
            _stateMachine = stateMachine;
        }

        public override void Show(object[] inData = null)
        {
            _audioService.PlaySound(SoundKeys.LOSESCREEN_SHOWN);
            
            homeButton.gameObject.SetActive(_hearthService.Count <= 0);
            restartButton.gameObject.SetActive(_hearthService.Count > 0);
            
            closeButton.OnClicked += GoToLobby;
            homeButton.OnClicked += GoToLobby;
            restartButton.OnClicked += RestartGame;
            
            base.Show(inData);
        }

        public override void Close()
        {
            closeButton.OnClicked -= GoToLobby;
            homeButton.OnClicked -= GoToLobby;
            restartButton.OnClicked -= RestartGame;
            base.Close();
        }

        private void GoToLobby()
        {
            _audioService.PlaySound(SoundKeys.TAP_OPEN);
            
            _stateMachine.Enter<LoadLobbyState, bool>(true);
            
            Close();
        }
        
        private void RestartGame()
        {
            _audioService.PlaySound(SoundKeys.TAP_OPEN);
            
            _stateMachine.Enter<LoadGameplayState, bool>(true);
            
            Close();
        }
    }
}