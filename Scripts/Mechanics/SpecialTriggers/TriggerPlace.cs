using System;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [System.Flags]
    public enum TriggerPlace
    {
        NONE = 0,
        GAMEFIELD = 1 << 0,
        SELECTED_LETTERS = 1 << 1,
        PERKS = 1 << 2
    }
}