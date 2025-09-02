using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using AtomicApps.Mechanics.Gameplay.Score;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public interface ITriggerListener
    {
        public TriggerWave TriggerWave { get; set; }
        public TriggerPlace TriggerPlace { get; set; }
        public ITriggerInitiator ConnectedInitiator { get; set; }
        public UniTask CheckTrigger(TriggerWave triggerWave, ITriggerInitiator initiator);
    }
}