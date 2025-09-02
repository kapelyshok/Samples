using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.Bonuses;
using AtomicApps.Mechanics.Gameplay.Levels;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class BonusesSelectionData
    {
        public BonusesManager BonusesManager;
        public LevelStagesManager LevelStagesManager;
        public List<ISelectableGameplayBonus> Bonuses;
        public int AdditionalTries;
        public int ReRollPrice;
        public int CompletedStageNumber;
        public int FreeReRollsCount;
        public RectTransform ActivePerksRect;
        public List<PerkItemView> PerkViews;
    }
}