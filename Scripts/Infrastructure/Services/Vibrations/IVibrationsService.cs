using Lofelt.NiceVibrations;

namespace AtomicApps
{
    public interface IVibrationsService
    {
        public void PlayVibration(HapticPatterns.PresetType type);
        public void ChangeVibrationEnabled(bool state);
        public bool IsEnabled { get; }
    }
}