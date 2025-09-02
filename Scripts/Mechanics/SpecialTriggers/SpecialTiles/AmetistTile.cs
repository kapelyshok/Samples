using AtomicApps.Infrastructure.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class AmetistTile : ABaseSpecialTile, ISecondWaveTriggerListener
    {
        [field:SerializeField]
        public CellCalculationStage CellCalculationStage { get; set; }
        [SerializeField]
        private int multiplier = 3;
        [SerializeField]
        private int cellIndexToFinish = 4;

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
            if(initiator != ConnectedInitiator) return;
            
            var selectedLetterCellView = ConnectedInitiator as SelectedLetterCellView;
            if (selectedLetterCellView == null) return;

            bool isSuccess = selectedLetterCellView.CellPosition <= cellIndexToFinish;

            if (isSuccess)
            {
                _audioService.PlaySound(SoundKeys.GEM2_ACTIVATED, .3f);

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