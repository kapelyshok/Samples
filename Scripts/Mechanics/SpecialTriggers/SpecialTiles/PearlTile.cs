using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class PearlTile : ABaseSpecialTile, ISecondWaveTriggerListener
    {
        [field:SerializeField]
        public CellCalculationStage CellCalculationStage { get; set; }
        [SerializeField]
        private int bonus = 5;

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
            _audioService.PlaySound(SoundKeys.GEM2_ACTIVATED, .3f);
            await AnimateTileSuccess();
            await selectedLetterCellView.AnimateTotalScoreValueChange(selectedLetterCellView.CurrentTotalCellScore + bonus);
        }
    }
}