using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.UI.Mechanics;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class ShowPopupButton : ButtonWrapper
    {
        [SerializeField]
        private string popupId;

        private IPopupService _popupService;
        private IAudioService _audioService;

        [Inject]
        private void Construct(IPopupService popupService, IAudioService audioService)
        {
            _audioService = audioService;
            _popupService = popupService;
        }

        public override void OnClickHandler()
        {
            _audioService.PlaySound(SoundKeys.TAP_OPEN);
            _popupService.ShowPopupAsync(popupId);
        }
    }
}