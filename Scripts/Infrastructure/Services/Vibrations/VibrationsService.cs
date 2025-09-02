using System;
using AtomicApps.Infrastructure.Services.SaveLoad;
using Lofelt.NiceVibrations;
using UnityEngine;
using Zenject;

namespace AtomicApps
{
    public class VibrationsService : MonoBehaviour, IVibrationsService
    {
        private VibrationsSaveData _saveData;
        private ISaveService _saveService;

        public bool IsEnabled => _saveData.IsEnabled;
        public event Action<bool> OnEnabledStateChanged;

        [Inject]
        private void Construct(ISaveService saveService)
        {
            _saveService = saveService;
        }
        
        private void Awake()
        {
            _saveData = _saveService.GetData<VibrationsSaveData>();
        }

        public void PlayVibration(HapticPatterns.PresetType type)
        {
            if (IsEnabled)
            {
                HapticPatterns.PlayPreset(type);
            }
        }

        public void ChangeVibrationEnabled(bool state)
        {
            _saveData.IsEnabled = state;
            _saveService.SaveDataImmediately(_saveData);
            OnEnabledStateChanged?.Invoke(state);
        }
    }
}
