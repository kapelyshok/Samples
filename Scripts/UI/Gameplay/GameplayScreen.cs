using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.Scpts.Mechanics.Lobby.Hearts;
using AtomicApps.UI.Mechanics;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Gameplay
{
    public class GameplayScreen : MonoBehaviour
    {
        [SerializeField]
        private CustomButton returnToLobbyButton;

        private IPopupService _popupService;

        [Inject]
        private void Construct(IPopupService popupService)
        {
            _popupService = popupService;
        }
        
        private void Awake()
        {
            returnToLobbyButton.OnClicked += TryExitGamefield;
        }

        private void OnDestroy()
        {
            returnToLobbyButton.OnClicked -= TryExitGamefield;
        }

        private void TryExitGamefield()
        {
            _popupService.ShowPopupAsync(PopupKeys.ARE_YOU_SURE_POPUP);
        }
    }
}
