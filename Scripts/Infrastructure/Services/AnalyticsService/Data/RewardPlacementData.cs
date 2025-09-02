using AtomicApps.Infrastructure.Services.AdsService;

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public class RewardPlacementData
    {
        public AdsPlacement PlacementType { get; private set; }
        public int Amount { get; private set; }

        public RewardPlacementData(AdsPlacement placementType, int amount)
        {
            PlacementType = placementType;
            Amount = amount;
        }

        public void IncreaseAmount() => Amount++;
    }
}
