using AtomicApps.Mechanics.Gameplay.LettersBag;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class BaseSpecialTileSO : ScriptableObject, ISelectableGameplayBonus
    {
        [field: SerializeField]
        public GameplayBonusType GameplayBonusType { get; set; }
        [field:SerializeField]
        public Tile Tile { get; set; }
        [field: SerializeField]
        public Rarity Rarity { get; set; }
        [field: SerializeField, SpritePreview] 
        public Sprite Icon { get; set; }
        [field: SerializeField]
        public string TextDetailed { get; set; }
        [field:SerializeField]
        public Color TextColor { get; set; }
        [field:SerializeField]
        public Gradient SuccessParticleColor { get; set; }
        [field:SerializeField]
        public Gradient SuccessParticleGlowColor { get; set; }
    }
}