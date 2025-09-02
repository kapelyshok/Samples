using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(ActionWordPerk), menuName = "ScriptableObjects/Perks/" + nameof(ActionWordPerk))]
    public class ActionWordPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int bonus = 15;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && word.EndsWith("ing", StringComparison.InvariantCultureIgnoreCase))
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }
    }
}