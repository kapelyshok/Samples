using System;
using AtomicApps.Infrastructure.Services.SaveLoad.Data;

namespace AtomicApps
{
    [Serializable]
    public class VibrationsSaveData : SavableData
    {
        public bool IsEnabled;
        public override void SetDefaultValues(object inData = null)
        {
            IsEnabled = true;
        }
    }
}