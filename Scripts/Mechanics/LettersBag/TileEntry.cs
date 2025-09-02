using System;

namespace AtomicApps.Mechanics.Gameplay.LettersBag
{
    [Serializable]
    public class TileEntry
    {
        public LetterEntry LetterEntry;
        public Tile Tile = Tile.DEFAULT;
    }
}