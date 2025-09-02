using System.Collections.Generic;
using System.Linq;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(TwinDropPerk), menuName = "ScriptableObjects/Perks/" + nameof(TwinDropPerk))]
    public class TwinDropPerk : PerkSO
    {
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var cells = _perksManager.SelectedLettersManager.GetCurrentlyFilledCells();

            var result = CheckRepeatedLetters(cells);
            
            if (cells.Count > 0 && result.HasValue)
            {
                result.Value.Item1.PlayHighlightAnimation();
                result.Value.Item2.PlayHighlightAnimation();
                await AnimateSuccess();
                
                var randomLetter = _perksManager.LettersBagManager.GetRandomLetterEntry();
                var tileEntryToAdd = new TileEntry() { Tile = Tile.PEARL, LetterEntry = randomLetter };
                var currentPerkView = _perksManager.TryGetViewForPerk(this);
            
                if (currentPerkView == null)
                {
                    Debug.LogError($"Can't find perk view for {this} perk! Smth went wrong!");
                    return;
                }
            
                RectTransform targetRectTransform = _perksManager.LettersBagManager.GetLettersBagView().GetComponent<RectTransform>();
                await _perksManager.FlyIconsManager.AnimateTileEntryFly(tileEntryToAdd, currentPerkView.GetTileShowPlace(),
                    targetRectTransform);
                _perksManager.LettersBagManager.AddTileEntryToBagAndRefresh(tileEntryToAdd);
                await UniTask.WaitForSeconds(0.3f);
            }
        }

        private (SelectedLetterCellView, SelectedLetterCellView)? CheckRepeatedLetters(List<SelectedLetterCellView> letters)
        {
            Dictionary<char, SelectedLetterCellView> seen = new Dictionary<char, SelectedLetterCellView>();

            foreach (SelectedLetterCellView view in letters)
            {
                foreach (char c in view.CurrentTileEntry.LetterEntry.Letter)
                {
                    if (c == '%')
                    {
                        return (letters.First(item=>item!=view), view);
                    }
                    
                    if (seen.TryGetValue(c, out SelectedLetterCellView previousView))
                    {
                        return (previousView, view);
                    }

                    seen[c] = view;
                }
            }

            return null;
        }

    }
}