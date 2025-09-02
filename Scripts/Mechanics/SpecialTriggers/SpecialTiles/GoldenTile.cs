using System;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Mechanics.Gameplay.Score;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class GoldenTile : ABaseSpecialTile, ISecondWaveTriggerListener
    {
        [field:SerializeField]
        public CellCalculationStage CellCalculationStage { get; set; }
        [SerializeField]
        private int multiplier = 1;

        private ScoreCalculationManager _scoreCalculationManager;
        private IAudioService _audioService;

        [Inject]
        private void Construct(ScoreCalculationManager scoreCalculationManager, IAudioService audioService)
        {
            _audioService = audioService;
            _scoreCalculationManager = scoreCalculationManager;
        }
        
        public bool IsActivatedToCell(ITriggerInitiator initiator)
        {
            return initiator == ConnectedInitiator;
        }
        
        public override async UniTask ProcessTile(ITriggerInitiator initiator)
        {
            if(initiator != ConnectedInitiator) return;
            
            _audioService.PlaySound(SoundKeys.GOLD_ACTIVATED, .3f);

            await AnimateTileSuccess();
            
            await _scoreCalculationManager.AddMultiplier(multiplier);
        }
    }
}