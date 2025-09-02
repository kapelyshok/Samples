using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Scpts.Mechanics.Lobby.Hearts;
using AtomicApps.UI.Mechanics;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class AreYouSurePopup : BasePopup
    {
        [SerializeField]
        private CustomButton returnToLobbyButton;

        private GameStateMachine _stateMachine;
        private IHearthService _hearthService;
        private IAudioService _audioService;

        [Inject]
        private void Construct(GameStateMachine stateMachine, IHearthService hearthService, IAudioService audioService)
        {
            _audioService = audioService;
            _hearthService = hearthService;
            _stateMachine = stateMachine;
        }
        
        private void Awake()
        {
            returnToLobbyButton.OnClicked += EnterLobbyState;
        }

        private void OnDestroy()
        {
            returnToLobbyButton.OnClicked -= EnterLobbyState;
        }

        private void EnterLobbyState()
        {
            _audioService.PlaySound(SoundKeys.TAP_CLOSE);

            _hearthService.SpendHearth();
            _stateMachine.Enter<LoadLobbyState, bool>(true);
            Close();
        }
    }
}