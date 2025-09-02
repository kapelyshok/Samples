using System;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public class FakeRewarded : BaseFakeAds
    {
        [SerializeField]
        private Button grantRewardButton;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private GameObject grantedText;
        [SerializeField]
        private GameObject notGrantedText;

        private bool _wasRewarded = false;
        
        public event Action OnRewardedGranted;
        public event Action OnRewardedSkipped;

        private void Awake()
        {
            grantRewardButton.onClick.AddListener(GrantReward);
            closeButton.onClick.AddListener(CloseRewardedAds);
        }

        private void OnDestroy()
        {
            grantRewardButton.onClick.RemoveListener(GrantReward);
            closeButton.onClick.RemoveListener(CloseRewardedAds);
        }

        public override void Show()
        {
            _wasRewarded = false;
            
            grantedText.SetActive(false);
            notGrantedText.SetActive(true);
            
            base.Show();
        }

        private void GrantReward()
        {
            _wasRewarded = true;
            grantedText.SetActive(true);
            notGrantedText.SetActive(false);
        }
        
        private void CloseRewardedAds()
        {
            if (_wasRewarded)
            {
                OnRewardedGranted?.Invoke();
            }
            else
            {
                OnRewardedSkipped?.Invoke();
            }
            
            Hide();
        }
    }
}