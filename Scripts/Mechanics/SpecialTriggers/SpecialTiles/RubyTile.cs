using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class RubyTile : ABaseSpecialTile
    {
        private SelectedLettersManager _selectedLettersManager;
        private IAudioService _audioService;

        [Inject]
        private void Construct(SelectedLettersManager selectedLettersManager, IAudioService audioService)
        {
            _audioService = audioService;
            _selectedLettersManager = selectedLettersManager;
        }
        
        public override async UniTask ProcessTile(ITriggerInitiator initiator)
        {
            var selectedLetterCellView = ConnectedInitiator as SelectedLetterCellView;
            
            if (selectedLetterCellView == null) return;
            
            var word = _selectedLettersManager.GetCurrentlyFilledCells();

            var tileIndex = word.IndexOf(selectedLetterCellView);
            
            if(tileIndex == -1) return;

            var calculationTimes = selectedLetterCellView.RepeatCalculationTimes;
            
            do
            {
                if (tileIndex - 1 >= 0)
                {
                    word[tileIndex - 1].ApplyRubyBlessing();
                }
                if (tileIndex + 1 < word.Count)
                {
                    word[tileIndex + 1].ApplyRubyBlessing();
                }

                _audioService.PlaySound(SoundKeys.GEM_ACTIVATED, .3f);
                await AnimateTileSuccess();

                calculationTimes--;
            } while (calculationTimes > 0);
        }
    }
}