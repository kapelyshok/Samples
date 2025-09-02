using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(SoftEndingPerk), menuName = "ScriptableObjects/Perks/" + nameof(SoftEndingPerk))]
    public class SoftEndingPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int bonus = 10;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && word.EndsWith("e", StringComparison.InvariantCultureIgnoreCase))
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }
    }
}