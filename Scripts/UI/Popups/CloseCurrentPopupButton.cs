using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.UI.Mechanics;
using AtomicApps.UI.Popups;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class CloseCurrentPopupButton : ButtonWrapper
    {
        [SerializeField]
        private BasePopup basePopup;

        private IAudioService _audioService;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public override void OnClickHandler()
        {
            _audioService.PlaySound(SoundKeys.TAP_CLOSE);

            basePopup.Close();
        }
    }
}
