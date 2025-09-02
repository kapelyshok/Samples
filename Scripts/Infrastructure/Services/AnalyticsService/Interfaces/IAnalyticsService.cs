using AtomicApps.Infrastructure.Services.AdsService;

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public interface IAnalyticsService
    {
        void Init();
        void TutorialStartStep(int tutorialIndex, int step);
        void TutorialCompleteStep(string blockName, int step);
        void ConnectionLost(int counter);
        void AdsStarted(AdsType type, AdsPlacement placementType);
        void AdsAvailable(AdsType type, AdsPlacement placementType, bool IsSuccess);
        void AdsWatched(AdsType type, AdsPlacement placementType);
        public void CompleteRacing(int carIndex, int raceIndex);
        public void StartRacing(int carIndex, int raceIndex);
        public void CarTuningBought(int carIndex, int tuningIndex);
        public void IdleGetChest(int carIndex, int chestIndex);
    }
}