using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(PreservePowerPerk), menuName = "ScriptableObjects/Perks/" + nameof(PreservePowerPerk))]
    public class PreservePowerPerk : PerkSO
    {
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            await AnimateSuccess();
            _perksManager.BoostersManager.ChangeIsRefreshingSpecialTiles(false);
            _wasActivated = true;
        }
    }
}