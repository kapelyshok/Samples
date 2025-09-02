using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(SmartEndingPerk), menuName = "ScriptableObjects/Perks/" + nameof(SmartEndingPerk))]
    public class SmartEndingPerk : PerkSO
    {
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;

            string additionalIng = word + "ing";
            string additionalEd = word + "ed";

            if (_perksManager.WordsDictionaryService.Contains(additionalIng))
            {
                await AnimateSuccess();
                var tile = new TileEntry()
                {
                    Tile = Tile.ING,
                    LetterEntry = new LetterEntry()
                };
                var view = _perksManager.SelectedLettersManager.TryAddSelectedLetter(tile);
                _perksManager.ScoreCalculationManager.AddCellToCurrentWordCells(view);
                return;
            }
            
            if (_perksManager.WordsDictionaryService.Contains(additionalEd))
            {
                await AnimateSuccess();
                var tile = new TileEntry()
                {
                    Tile = Tile.ED,
                    LetterEntry = new LetterEntry()
                };
                var view = _perksManager.SelectedLettersManager.TryAddSelectedLetter(tile);
                _perksManager.ScoreCalculationManager.AddCellToCurrentWordCells(view);
                return;
            }
        }
    }
}