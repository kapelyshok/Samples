using System;

namespace AtomicApps.Infrastructure.Services.SaveLoad.Data
{
    public interface ISavable
    {
        public string LastTimeSavedUtc { get; set; }
        public event Action OnDataChanged;
        public void NotifyChanges();
        
        /// <summary>
        /// Automatically calls when you delete this data from SaveService
        /// </summary>
        /// <param name="inData"></param>
        public void SetDefaultValues(object inData = null);
    }
}