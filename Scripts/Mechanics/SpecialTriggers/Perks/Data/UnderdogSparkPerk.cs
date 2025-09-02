using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(UnderdogSparkPerk), menuName = "ScriptableObjects/Perks/" + nameof(UnderdogSparkPerk))]
    public class UnderdogSparkPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int scoreToConsiderLowValue = 2;
        [SerializeField]
        private int amountOfLowValueLettersToTrigger = 3;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var selectedCells = _perksManager.SelectedLettersManager.GetCurrentlyFilledCells();
            List<SelectedLetterCellView> lowValueCells = new List<SelectedLetterCellView>();
            foreach (var cell in selectedCells)
            {
                if (cell.CurrentTileEntry.LetterEntry.Points <= scoreToConsiderLowValue)
                {
                    lowValueCells.Add(cell);
                }
            }

            if (lowValueCells.Count < amountOfLowValueLettersToTrigger)
            {
                return;
            }

            var tasks = new List<UniTask>();
            foreach (var cell in lowValueCells)
            {
                tasks.Add(cell.PlayHighlightAnimation());
            }
            await UniTask.WhenAll(tasks);
            
            var cells = _perksManager.GamefieldLettersManager.GetGamefieldViews();
            
            List<GamefieldLetterView> availableViews = new List<GamefieldLetterView>();
            
            foreach (var cell in cells)
            {
                if(!cell.IsEmpty() && !cell.IsLocked() && cell.CurrentTileEntry.Tile == Tile.DEFAULT) availableViews.Add(cell);
            }

            if (availableViews.Count <= 0)
            {
                return;
            }
            
            await AnimateSuccess();
            
            var targetTile = availableViews[Random.Range(0, availableViews.Count)];
            var newTileEntry = targetTile.CurrentTileEntry;
            newTileEntry.Tile = Tile.RUBY;
            await targetTile.ChangeTileEntry(newTileEntry);
        }
    }
}