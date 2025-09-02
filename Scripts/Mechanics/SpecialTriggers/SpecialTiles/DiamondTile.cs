using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Mechanics.Gameplay.Score;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class DiamondTile : ABaseSpecialTile
    {
        private IAudioService _audioService;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public override async UniTask ProcessTile(ITriggerInitiator initiator)
        {
            var gamefieldLetterView = ConnectedInitiator as GamefieldLetterView;
            if (gamefieldLetterView == null) return;
            gamefieldLetterView.CurrentTileEntry.LetterEntry.Points += 5;
            _audioService.PlaySound(SoundKeys.GEM_ACTIVATED, .3f);
            await AnimateTileSuccess();
            await gamefieldLetterView.UpdateDisplayData();
            DisableGlow();
        }
    }
}