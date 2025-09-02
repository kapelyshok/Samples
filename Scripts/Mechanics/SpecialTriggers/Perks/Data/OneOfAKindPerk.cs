using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(OneOfAKindPerk), menuName = "ScriptableObjects/Perks/" + nameof(OneOfAKindPerk))]
    public class OneOfAKindPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int bonus = 10;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && !CheckRepeatedLetters(word))
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }

        private bool CheckRepeatedLetters(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;

            HashSet<char> seen = new HashSet<char>();

            foreach (char c in word.ToLowerInvariant())
            {
                if (seen.Contains(c))
                {
                    return true;
                }
                seen.Add(c);
            }

            return false;
        }
    }
}