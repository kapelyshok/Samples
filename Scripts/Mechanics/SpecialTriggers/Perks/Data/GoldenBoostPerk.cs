using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(GoldenBoostPerk), menuName = "ScriptableObjects/Perks/" + nameof(GoldenBoostPerk))]
    public class GoldenBoostPerk : PerkSO
    {
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var cells = _perksManager.GamefieldLettersManager.GetGamefieldViews();
            List<GamefieldLetterView> availableViews = new List<GamefieldLetterView>();
            
            foreach (var cell in cells)
            {
                if(!cell.IsEmpty() && cell.CurrentTileEntry.Tile == Tile.DEFAULT) availableViews.Add(cell);
            }

            if (availableViews.Count <= 0)
            {
                return;
            }
            
            await AnimateSuccess();
            
            var targetTile = availableViews[Random.Range(0, availableViews.Count)];
            var newTileEntry = targetTile.CurrentTileEntry;
            newTileEntry.Tile = Tile.GOLDEN;
            await targetTile.ChangeTileEntry(newTileEntry);
        }
    }
}