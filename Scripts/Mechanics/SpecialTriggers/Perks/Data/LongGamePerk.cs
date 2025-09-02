using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(LongGamePerk), menuName = "ScriptableObjects/Perks/" + nameof(LongGamePerk))]
    public class LongGamePerk : PerkSO
    {
        [Space]
        [SerializeField]
        private int bonus = 5;
        [SerializeField]
        private int lettersAmountToTrigger = 5;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && word.Length >= lettersAmountToTrigger)
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }
    }
}