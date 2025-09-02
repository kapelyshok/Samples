using System;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.UI.Mechanics;
using UnityEngine;
using Zenject;

namespace AtomicApps._Project.Code.Scripts.UI.Utils
{
    [RequireComponent(typeof(CustomButton))]
    public class CustomToggleButton : MonoBehaviour
    {
        [SerializeField] 
        private GameObject activeState;

        [SerializeField] 
        private GameObject inactiveState;
        
        private CustomButton customButton;
        private bool isOn = true;
        private IAudioService _audioService;

        public event Action<bool> OnStateChanged;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void Start()
        {
            customButton = GetComponent<CustomButton>();
            customButton.OnClicked += Toggle;
            UpdateView();
        }

        private void OnDestroy()
        {
            customButton.OnClicked -= Toggle;
        }

        public void Toggle()
        {
            _audioService.PlaySound(SoundKeys.MENU_CHANGE_SETTINGS);
            isOn = !isOn;
            OnStateChanged?.Invoke(isOn);
            UpdateView();
        }
        
        public void SetState(bool state)
        {
            isOn = state;
            OnStateChanged?.Invoke(isOn);
            UpdateView();
        }

        private void UpdateView()
        {
            if (activeState != null) activeState.SetActive(isOn);
            if (inactiveState != null) inactiveState.SetActive(!isOn);
        }
    }
}
