using System.Linq;
using AtomicApps.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(HardStopPerk), menuName = "ScriptableObjects/Perks/" + nameof(HardStopPerk))]
    public class HardStopPerk : PerkSO
    {       
        [Space]
        [SerializeField]
        private int bonus = 5;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && !word.Last().IsVowel())
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }
    }
}