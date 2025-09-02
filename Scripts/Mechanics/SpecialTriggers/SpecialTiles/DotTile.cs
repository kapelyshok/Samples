using System.Linq;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Mechanics.Gameplay.Score;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class DotTile : ABaseSpecialTile, ISecondWaveTriggerListener
    {
        [field:SerializeField]
        public CellCalculationStage CellCalculationStage { get; set; }
        [SerializeField]
        private int multiplier = 1;

        private SelectedLettersManager _selectedLettersManager;
        private ScoreCalculationManager _scoreCalculationManager;

        [Inject]
        private void Construct(SelectedLettersManager selectedLettersManager,
            ScoreCalculationManager scoreCalculationManager)
        {
            _scoreCalculationManager = scoreCalculationManager;
            _selectedLettersManager = selectedLettersManager;
        }
        
        public bool IsActivatedToCell(ITriggerInitiator initiator)
        {
            return initiator == ConnectedInitiator;
        }

        public override void Activate(TileEntryView tileEntryView)
        {
            base.Activate(tileEntryView);
            tileEntryView.LetterText.text += ".";
        }

        public override void Disable(TileEntryView tileEntryView)
        {
            base.Disable(tileEntryView);
            if (tileEntryView.LetterText.text.Contains("."))
            {
                tileEntryView.LetterText.text = tileEntryView.LetterText.text.Replace(".", "");
            }
        }

        public override async UniTask ProcessTile(ITriggerInitiator initiator)
        {
            if(initiator != ConnectedInitiator) return;
            
            var selectedLetterCellView = ConnectedInitiator as SelectedLetterCellView;
            
            if (selectedLetterCellView == null) return;

            bool isSuccess = IsLastLetter(initiator);

            if (isSuccess)
            {
                await AnimateTileSuccess();
                
                await _scoreCalculationManager.AddMultiplier(multiplier);
            }
        }

        private bool IsLastLetter(ITriggerInitiator initiator)
        {
            var word = _selectedLettersManager.GetCurrentlyFilledCells();
            
            if(word == null) return false;
            
            return word.Last() == (SelectedLetterCellView)initiator;
        }
    }
}