using System.Linq;
using AtomicApps.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(VowelFlowPerk), menuName = "ScriptableObjects/Perks/" + nameof(VowelFlowPerk))]
    public class VowelFlowPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int amountOfVowelsToTrigger = 3;
        [SerializeField]
        private int bonus = 10;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && IsEnoughVowels(word))
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }

        private bool IsEnoughVowels(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return false;

            int count = word.Count(c => c.IsVowel());
            return count >= amountOfVowelsToTrigger;
        }
    }
}