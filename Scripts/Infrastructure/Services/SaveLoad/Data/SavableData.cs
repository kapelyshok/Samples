using System;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.SaveLoad.Data
{
    [Serializable]
    public abstract class SavableData : ISavable
    {
        [field: SerializeField] public string LastTimeSavedUtc { get; set; }
        
        public event Action OnDataChanged;
        
        public void NotifyChanges()
        {
            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Automatically calls during first initialization and when you delete this data from SaveService
        /// </summary>
        /// <param name="inData"></param>
        public abstract void SetDefaultValues(object inData = null);
    }
}
