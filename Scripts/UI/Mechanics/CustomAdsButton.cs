using System;
using AtomicApps.Infrastructure.Services.AdsService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Mechanics
{
    public class CustomAdsButton : ButtonWrapper
    {
        [SerializeField]
        private int secondsToCheckFroAds = 5;
        [SerializeField]
        private GameObject loader;
        
        private IAdsService _adsService;
        public event Action OnRewardGranted;

        [Inject]
        private void Construct(IAdsService adsService)
        {
            _adsService = adsService;
        }

        protected void Start()
        {
            CheckVisuals();

            OnClicked += CheckForRewardedAds;
        }

        private void CheckForRewardedAds()
        {
            CheckVisuals();
            
            if (_adsService.IsRewardedReady())
            {
                _adsService.OnRewardedGranted += GrantReward;
                _adsService.OnRewardedSkipped += SkipReward;
                _adsService.ShowRewarded();
            }
        }

        private void CheckVisuals()
        {
            if (!_adsService.IsRewardedReady())
            {
                if (loader != null)
                {
                    loader.gameObject.SetActive(true);
                }
                
                ChangeInteractable(false);
                StartCheckingForAds();
            }
            else
            {
                if (loader != null)
                {
                    loader.gameObject.SetActive(false);
                }
                
                ChangeInteractable(true);
            }
        }

        private void OnDestroy()
        {
            SkipReward();
        }

        private void GrantReward()
        {
            OnRewardGranted?.Invoke();
        }
        
        private void SkipReward()
        {
            _adsService.OnRewardedGranted -= GrantReward;
            _adsService.OnRewardedSkipped -= SkipReward;
        }

        private async UniTask StartCheckingForAds()
        {
            await UniTask.WaitForSeconds(secondsToCheckFroAds);
            CheckForRewardedAds();
        }

        public override void OnClickHandler()
        {
            
        }
    }
}