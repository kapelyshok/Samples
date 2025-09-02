namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public enum TriggerWave
    {
        /// <summary>
        /// When Submit button is pressed
        /// </summary>
        SUBMIT_PRESSED = 0,
        
        /// <summary>
        /// When calculates every tile score
        /// </summary>
        TILE_CALCULATION = 1,
        
        /// <summary>
        /// When every tile score is calculated
        /// </summary>
        WORD_CALCULATED = 2,
        
        /// <summary>
        /// When bonuses selection popup is closed
        /// </summary>
        AFTER_BONUSES_SELECTION = 3,   
        
        /// <summary>
        /// When used any booster
        /// </summary>
        AFTER_BOOSTER_USED = 4,
        
        /// <summary>
        /// For default tiles
        /// </summary>
        NONE = 99
    }
}