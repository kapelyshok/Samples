using System;
using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Infrastructure;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.Scpts.Mechanics.Lobby.Hearts;
using AtomicApps.UI.Mechanics;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Lobby
{
    public class LobbyScreen : MonoBehaviour
    {
        [SerializeField]
        private CustomButton playButton;

        private GameStateMachine _stateMachine;
        private IHearthService _hearthService;
        private IAudioService _audioService;
        private IPopupService _popupService;

        [Inject]
        private void Construct(GameStateMachine stateMachine, IHearthService hearthService, IAudioService audioService,
            IPopupService popupService)
        {
            _popupService = popupService;
            _audioService = audioService;
            _hearthService = hearthService;
            _stateMachine = stateMachine;
        }
        
        private void Awake()
        {
            playButton.OnClicked += EnterGameplayState;
        }

        private void OnDestroy()
        {
            playButton.OnClicked -= EnterGameplayState;
        }

        private void EnterGameplayState()
        {
            _audioService.PlaySound(SoundKeys.TAP_OPEN);
            if (_hearthService.Count > 0)
            {
                _audioService.PlaySound(SoundKeys.LEVEL_START);
                _stateMachine.Enter<LoadGameplayState, bool>(true);
            }
            else
            {
                _popupService.ShowPopupAsync(PopupKeys.OUT_OF_LIVES_POPUP);
            }
        }
    }
}
