using System;
using AtomicApps.Infrastructure.Services.SaveLoad.Data;

namespace AtomicApps.Infrastructure.Services.Audio.Data
{
    [Serializable]
    public class AudioSaveData : SavableData
    {
        public bool IsSoundOn;
        public bool IsMusicOn;
        public float SoundsVolume = 1f;
        public float MusicVolume = 1f;
        
        public override void SetDefaultValues(object inData = null)
        {
            IsSoundOn = true;
            IsMusicOn = true;
            SoundsVolume = 1f;
            MusicVolume = 1f;
            
            NotifyChanges();
        }
    }
}
