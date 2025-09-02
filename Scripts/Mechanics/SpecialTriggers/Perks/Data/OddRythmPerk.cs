using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(OddRythmPerk), menuName = "ScriptableObjects/Perks/" + nameof(OddRythmPerk))]
    public class OddRythmPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int bonus = 15;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && word.Length % 2 != 0)
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }
    }
}