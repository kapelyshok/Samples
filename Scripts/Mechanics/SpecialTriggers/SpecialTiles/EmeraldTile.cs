using System.Threading.Tasks;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class EmeraldTile : ABaseSpecialTile, ISecondWaveTriggerListener
    {
        [field:SerializeField]
        public CellCalculationStage CellCalculationStage { get; set; }
        [SerializeField]
        private int successPercent = 25;
        [SerializeField]
        private int multiplier = 5;

        private IAudioService _audioService;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public bool IsActivatedToCell(ITriggerInitiator initiator)
        {
            return initiator == ConnectedInitiator;
        }
        
        public override async UniTask ProcessTile(ITriggerInitiator initiator)
        {
            var selectedLetterCellView = ConnectedInitiator as SelectedLetterCellView;
            if (selectedLetterCellView == null) return;

            bool isSuccess = Random.Range(0, 100) <= successPercent;
            
            if (isSuccess)
            {
                _audioService.PlaySound(SoundKeys.GEM_ACTIVATED, .3f);
                await AnimateTileSuccess();
                await selectedLetterCellView.AnimateTotalScoreValueChange(selectedLetterCellView.CurrentTotalCellScore * multiplier);
            }
            else
            {
                _audioService.PlaySound(SoundKeys.GEM_EMERALD_FAIL);
                await AnimateTileFail();
            }
        }
    }
}