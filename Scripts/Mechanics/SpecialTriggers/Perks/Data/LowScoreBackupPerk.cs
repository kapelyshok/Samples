using AtomicApps.Mechanics.Gameplay.LettersBag;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(LowScoreBackupPerk), menuName = "ScriptableObjects/Perks/" + nameof(LowScoreBackupPerk))]
    public class LowScoreBackupPerk : PerkSO
    {        
        [Space]
        [SerializeField]
        private int scoreToTrigger = 20;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var score = _perksManager.ScoreCalculationManager.GetCurrentCalculatedTotalScore();
            if (score < scoreToTrigger)
            {
                await AnimateSuccess();
                
                var randomLetter = _perksManager.LettersBagManager.GetRandomLetterEntry();
                var tileEntryToAdd = new TileEntry() { Tile = Tile.AMETIST, LetterEntry = randomLetter };
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
    }
}