namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public interface ISecondWaveTriggerListener : ITriggerListener
    {
        public CellCalculationStage CellCalculationStage { get; set; }

        public bool IsActivatedToCell(ITriggerInitiator initiator);
    }
}