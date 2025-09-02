using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(SecondChancePerk), menuName = "ScriptableObjects/Perks/" + nameof(SecondChancePerk))]
    public class SecondChancePerk : PerkSO
    {
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            await AnimateSuccess();
            _perksManager.BonusesManager.AddFreeReRollsCount(1);
            _wasActivated = true;
        }
    }
}