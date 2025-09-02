using System;
using AtomicApps._Project.Code.Scripts.UI.Utils;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.UI.Mechanics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class SettingsPopup : BasePopup
    {
        private const string EMAIL_ADDRESS = "atomicplayapps@gmail.com";
        private const string PRIVACY_POLICY = "https://atomicplayapps-web.web.app/sword_privacy_policy.html";
        private const string EMAIL_SUBJECT = "Game Support";
        private const string EMAIL_BODY = "Hello, I need help with";

        [SerializeField]
        private CustomButton supportButton;
        [SerializeField]
        private CustomButton privacyPolicyButton;
        [SerializeField]
        private Slider musicSlider;
        [SerializeField]
        private Slider soundsSlider;
        [SerializeField]
        private CustomToggleButton notificationToggle;
        [SerializeField]
        private CustomToggleButton vibrationsToggle;
        
        private IAudioService _audioService;
        private IVibrationsService _vibrationsService;

        [Inject]
        private void Construct(IAudioService audioService, IVibrationsService vibrationsService)
        {
            _vibrationsService = vibrationsService;
            _audioService = audioService;
        }
        
        public override void Show(object[] inData = null)
        {
            musicSlider.value = _audioService.MusicVolume;
            soundsSlider.value = _audioService.SoundsVolume;
            vibrationsToggle.SetState(_vibrationsService.IsEnabled);
            
            musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            soundsSlider.onValueChanged.AddListener(ChangeSoundsVolume);
            supportButton.OnClicked += OpenSupportMail;
            privacyPolicyButton.OnClicked += OpenPrivacyPolicy;
            notificationToggle.OnStateChanged += ChangeNotificationState;
            vibrationsToggle.OnStateChanged += ChangeVibrationsState;
            base.Show(inData);
        }

        public override void Close()
        {
            musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
            soundsSlider.onValueChanged.RemoveListener(ChangeSoundsVolume);
            supportButton.OnClicked -= OpenSupportMail;
            privacyPolicyButton.OnClicked -= OpenPrivacyPolicy;
            notificationToggle.OnStateChanged -= ChangeNotificationState;
            vibrationsToggle.OnStateChanged -= ChangeVibrationsState;
            base.Close();
        }

        private void ChangeMusicVolume(float value)
        {
            _audioService.ChangeMusicVolume(value);
        }

        private void ChangeSoundsVolume(float value)
        {
            _audioService.ChangeSoundVolume(value);
        }

        private void ChangeVibrationsState(bool state)
        {
            _vibrationsService.ChangeVibrationEnabled(state);
        }

        private void ChangeNotificationState(bool state)
        {
            
        }

        private void OpenPrivacyPolicy()
        {
            Application.OpenURL(PRIVACY_POLICY);
        }
        
        private void OpenSupportMail()
        {
            string email = $"mailto:{EMAIL_ADDRESS}?subject={Escape(EMAIL_SUBJECT)}&body={Escape(EMAIL_BODY)}";
            Application.OpenURL(email);
        }

        private string Escape(string s)
        {
            return WWW.EscapeURL(s).Replace("+", "%20");
        }
    }
}