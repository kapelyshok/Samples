using AtomicApps.Mechanics.Gameplay.Gamefield;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers.Boosters;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(PatiencePerk), menuName = "ScriptableObjects/Perks/" + nameof(PatiencePerk))]
    public class PatiencePerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int wordInARowToTrigger = 4;
        
        private int _currentWordsInARow = 0;

        protected override void InternalInitialize()
        {
            _currentWordsInARow = 0;
            _perksManager.BoostersManager.OnBoosterUsed += CheckBoosterUsed;
            base.InternalInitialize();
        }

        public override void Dispose()
        {
            _perksManager.BoostersManager.OnBoosterUsed -= CheckBoosterUsed;
            base.Dispose();
        }

        private void CheckBoosterUsed(BoosterType boosterType)
        {
            if (boosterType == BoosterType.REFRESH_BOOSTER)
            {
                _currentWordsInARow = 0;
            }
        }

        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            _currentWordsInARow++;
            
            if (_currentWordsInARow < wordInARowToTrigger)
            {
                return;
            }
            
            await AnimateSuccess();
            _currentWordsInARow = 0;
            
            var randomLetter = _perksManager.LettersBagManager.GetRandomLetterEntry();
            var tileEntryToAdd = new TileEntry() { Tile = Tile.DIAMOND, LetterEntry = randomLetter };
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