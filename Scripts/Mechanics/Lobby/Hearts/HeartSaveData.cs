using System;
using AtomicApps.Infrastructure.Services.SaveLoad.Data;

namespace AtomicApps.Scpts.Mechanics.Lobby.Hearts
{
    [Serializable]
    public class HeartSaveData : SavableData
    {
        public float FreeHealthLeftTime;
        public DateTime LastCloseTime;
        public float RemainingTimeForNextHeart;
        
        public override void SetDefaultValues(object inData = null)
        {
            if (inData != null && inData is HeartSaveData)
            {
                var data = (HeartSaveData)inData;
                FreeHealthLeftTime = data.FreeHealthLeftTime;
                LastCloseTime = data.LastCloseTime;
                RemainingTimeForNextHeart = data.RemainingTimeForNextHeart;
            }
            else
            {
                FreeHealthLeftTime = 0;
                LastCloseTime = DateTime.Now;
                RemainingTimeForNextHeart = 0;
            }
        }
    }
}