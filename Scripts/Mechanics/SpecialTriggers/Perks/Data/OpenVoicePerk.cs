using System.Linq;
using System.Threading.Tasks;
using AtomicApps.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(OpenVoicePerk), menuName = "ScriptableObjects/Perks/" + nameof(OpenVoicePerk))]
    public class OpenVoicePerk : PerkSO
    {        
        [Space]
        [SerializeField]
        private int bonus = 5;
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var word = _perksManager.SelectedLettersManager.LastSubmittedWord;
            
            if (!string.IsNullOrEmpty(word) && word.IsFirstLetterVowel())
            {
                await AnimateSuccess();
                await _perksManager.ScoreCalculationManager.AddPointsToTotalScoreWithAnimation(bonus);
                await UniTask.WaitForSeconds(.3f);
            }
        }
    }
}