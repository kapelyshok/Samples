using System;

namespace AtomicApps.Mechanics.Lobby.Hearts
{
    public static class GivenBonusForTimeCalculation
    {
        public static int Calculate(
            DateTime lastCloseDate,
            float remainingTimeForNextHeartLoad,
            int replenishIntervalInSeconds,
            out float remainingTimeForNextHeart)
        {
            int elapsedSec = (int)Math.Max(0, (DateTime.Now - lastCloseDate).TotalSeconds);

            int bonuses = 0;

            if (elapsedSec < remainingTimeForNextHeartLoad)
            {
                remainingTimeForNextHeart = remainingTimeForNextHeartLoad - elapsedSec;
                return 0;
            }

            bonuses++; 
            int afterFirstSec = elapsedSec - (int)remainingTimeForNextHeartLoad;

            bonuses += afterFirstSec / replenishIntervalInSeconds;

            int remainder = afterFirstSec % replenishIntervalInSeconds;

            remainingTimeForNextHeart = (remainder == 0)
                ? replenishIntervalInSeconds
                : (replenishIntervalInSeconds - remainder);

            return bonuses;
        }
    }
}