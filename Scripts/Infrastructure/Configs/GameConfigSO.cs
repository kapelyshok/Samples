using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Mechanics.Gameplay.Levels;
using UnityEngine;

namespace AtomicApps.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = nameof(GameConfigSO), menuName = "ScriptableObjects/Configs/" + nameof(GameConfigSO))]

    public class GameConfigSO : ScriptableObject
    {
        public int DefaultWordAttempthsForStage;
        public int WordAttempthsBonusForCompletingStage;
        public int ReRollBonusesForCompletingStagePrice;
        public int MaxHearts = 5;
        public int HeartCooldownSeconds = 1200;
        public int KeepPlayingBonusSubmits = 4;
        public int KeepPlayingBonusSubmitsCoinPrice = 100;
        public int RefillHeartsCoinsPrice = 100;
        public CurrenciesConfigSO CurrenciesConfig;
        public LevelsConfigSO LevelsConfig;
        public BoosterPricesConfigSO BoosterPricesConfig;
        public BoostersUnlockingDataSO BoostersUnlockingData;
        public GameHintsConfigSO GameHintsConfig;
        public TutorialsConfigSO TutorialsConfigSo;
    }
}