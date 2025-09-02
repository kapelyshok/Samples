using System;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class TriggerRegister : MonoBehaviour
    {
        [field:SerializeField]
        public TriggerPlace TriggerPlace { get; set; }
        
        protected TriggersManager _triggersManager;

        [Inject]
        protected virtual void Construct(TriggersManager triggersManager)
        {
            _triggersManager = triggersManager;
        }

        protected void RegisterTrigger(ITriggerListener triggerListener)
        {
            if (HasAnyCommonFlag(TriggerPlace, triggerListener.TriggerPlace))
            {
                _triggersManager.AddTrigger(triggerListener);
            }
        }

        protected void RemoveTrigger(ITriggerListener iITriggerListener)
        {
            if (HasAnyCommonFlag(TriggerPlace, iITriggerListener.TriggerPlace))
            {
                _triggersManager.RemoveTrigger(iITriggerListener);
            }
        }
        
        protected static bool HasAnyCommonFlag(TriggerPlace a, TriggerPlace b)
        {
            return ToList(a).Any(flag => b.HasFlag(flag));
        }
        
        protected static List<TriggerPlace> ToList(TriggerPlace flags)
        {
            return Enum.GetValues(typeof(TriggerPlace))
                .Cast<TriggerPlace>()
                .Where(flag => flag != TriggerPlace.NONE && flags.HasFlag(flag))
                .ToList();
        }

        protected TriggerPlace FromList(List<TriggerPlace> list)
        {
            TriggerPlace result = TriggerPlace.NONE;
            foreach (TriggerPlace place in list)
            {
                result |= place;
            }
            return result;
        }
    }
}