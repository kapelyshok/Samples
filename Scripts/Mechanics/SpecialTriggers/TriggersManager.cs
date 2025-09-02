using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VInspector;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class TriggersManager : MonoBehaviour
    {
        private Dictionary<TriggerWave, List<ITriggerListener>> _triggers = new();

        public List<ISecondWaveTriggerListener> GetCurrentInitiatorListeners(TriggerWave triggerWave, ITriggerInitiator initiator)
        {
            var triggerListeners = new List<ISecondWaveTriggerListener>();
            if (!_triggers.ContainsKey(triggerWave) || _triggers[triggerWave] == null)
            {
                return triggerListeners;
            }
            
            List<ITriggerListener> triggersCopy = new List<ITriggerListener>(_triggers[triggerWave]);

            foreach (ITriggerListener trigger in triggersCopy)
            {
                if (trigger is ISecondWaveTriggerListener secondTriggerListener)
                {
                    if (secondTriggerListener.IsActivatedToCell(initiator))
                    {
                        triggerListeners.Add(secondTriggerListener);
                    }
                }
            }
            
            return triggerListeners;
        }

        public async UniTask ProcessTriggersWave(TriggerWave triggerWave, ITriggerInitiator initiator)
        {
            if (!_triggers.ContainsKey(triggerWave) || _triggers[triggerWave] == null)
            {
                return;
            }

            List<ITriggerListener> triggersCopy = new List<ITriggerListener>(_triggers[triggerWave]);

            foreach (ITriggerListener trigger in triggersCopy)
            {
                await trigger.CheckTrigger(triggerWave, initiator);
            }
        }
        
        public async UniTask ProcessTriggersWave(TriggerWave triggerWave, ITriggerInitiator initiator, float secondsDelay)
        {
            await UniTask.WaitForSeconds(secondsDelay);
            
            if (!_triggers.ContainsKey(triggerWave) || _triggers[triggerWave] == null)
            {
                return;
            }

            List<ITriggerListener> triggersCopy = new List<ITriggerListener>(_triggers[triggerWave]);

            foreach (ITriggerListener trigger in triggersCopy)
            {
                await trigger.CheckTrigger(triggerWave, initiator);
            }
        }

        public void AddTrigger(ITriggerListener triggerListener)
        {
            if (triggerListener.TriggerWave == TriggerWave.NONE) return;

            if (!_triggers.ContainsKey(triggerListener.TriggerWave))
            {
                _triggers.Add(triggerListener.TriggerWave, new());
            }

            List<ITriggerListener> container = _triggers[triggerListener.TriggerWave];

            if (triggerListener is ABaseSpecialTile specialTile)
            {
                if (specialTile.ConnectedInitiator is SelectedLetterCellView selectedLetterCellView)
                {
                    int newCellPosition = selectedLetterCellView.CellPosition;
                    int insertIndex = container.Count;

                    for (int i = 0; i < container.Count; i++)
                    {
                        if (container[i] is ABaseSpecialTile otherTile &&
                            otherTile.ConnectedInitiator is SelectedLetterCellView otherCellView)
                        {
                            int otherPosition = otherCellView.CellPosition;

                            if (newCellPosition < otherPosition)
                            {
                                insertIndex = i;
                                break;
                            }
                        }
                        else
                        {
                            insertIndex = i;
                            break;
                        }
                    }

                    container.Insert(insertIndex, triggerListener);
                }
                else
                {
                    container.Add(triggerListener); 
                }
            }
            else
            {
                container.Add(triggerListener);
            }

            Debug.Log($"Added trigger {triggerListener} to wave {triggerListener.TriggerWave}");
        }
        
        public void RemoveTrigger(ITriggerListener iITriggerListener)
        {
            if (!_triggers.ContainsKey(iITriggerListener.TriggerWave) || _triggers[iITriggerListener.TriggerWave] == null)
            {
                return;
            }
            
            Debug.Log($"Removed trigger {iITriggerListener} to wave {iITriggerListener.TriggerWave}");
            _triggers[iITriggerListener.TriggerWave].Remove(iITriggerListener);
        }

        [Button]
        public void PrintAllCurrentTriggers()
        {
            foreach (var kvp in _triggers)
            {
                TriggerWave wave = kvp.Key;
                List<ITriggerListener> listeners = kvp.Value;

                Debug.Log($"--- TriggerWave: {wave} ---");

                foreach (ITriggerListener listener in listeners)
                {
                    string name = listener.ToString();

                    if (listener is ABaseSpecialTile specialTile &&
                        specialTile.ConnectedInitiator is SelectedLetterCellView cellView)
                    {
                        int cellPosition = cellView.CellPosition;
                        Debug.Log($"Listener: {name} | CellPosition: {cellPosition}");
                    }
                    else
                    {
                        Debug.Log($"Listener: {name}");
                    }
                }
            }
        }
    }
}
