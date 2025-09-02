using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public interface ISelectableGameplayBonus
    {
        public GameplayBonusType GameplayBonusType { get; set; }
        public Rarity Rarity { get; set; }
        public Sprite Icon { get; set; }
        public string TextDetailed { get; set; }
    }
}