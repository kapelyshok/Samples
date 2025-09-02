using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public class AdsService : MonoBehaviour, IAdsService
    {
        [SerializeField]
        private BaseAdsProvider adsProvider;
        [SerializeField]
        private bool showBanner = false;
        
        public BannerStateData CurrentBannerState { get; private set; }
        public event Action OnRewardedGranted;
        public event Action OnRewardedSkipped;
        public event Action<BannerStateData> OnBannerStateChanged;

        /*private IAnalyticsService _analyticsService;

        [Inject]
        private void Construct(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }*/

        private async void Awake()
        {
            await Init();
        }

        public async UniTask Init()
        {
            CurrentBannerState = new BannerStateData() { IsBannerActive = false, BannerHeight = 0 };
            
            adsProvider.Initialize();

            adsProvider.OnRewardedGranted += NotifyRewardedGranted;
            adsProvider.OnRewardedSkipped += NotifyRewardedSkipped;

            if (showBanner)
            {
                EnableBanner();
            }
        }

        private void OnDestroy()
        {
            adsProvider.OnRewardedGranted -= NotifyRewardedGranted;
            adsProvider.OnRewardedSkipped -= NotifyRewardedSkipped;
        }

        public bool IsInterstitialReady()
        {
            return adsProvider.IsInterstitialReady();
        }

        public void ShowInterstitial()
        {
            if (adsProvider.ShowInterstitial())
            {
                //Successfully showed interstitial ads analytics event
                
            }
            else
            {
                //Interstitial ads requested but failed to show analytics event
                
            }
        }

        public bool IsRewardedReady()
        {
            return adsProvider.IsRewardedReady();
        }

        public void ShowRewarded()
        {
            if (adsProvider.ShowRewarded())
            {
                //Successfully showed rewarded ads analytics event
                
            }
            else
            {
                //Rewarded ads requested but failed to show analytics event
                
            }
        }

        public void EnableBanner()
        {
            adsProvider.InitializeBanner();
            CurrentBannerState.IsBannerActive = true;
            CurrentBannerState.BannerHeight = adsProvider.GetBannerHeight();
            OnBannerStateChanged?.Invoke(CurrentBannerState);
        }
        
        public void DisableBanner()
        {
            adsProvider.DisableBanner();
            CurrentBannerState.IsBannerActive = false;
            OnBannerStateChanged?.Invoke(CurrentBannerState);
        }

        private void NotifyRewardedSkipped()
        {
            OnRewardedSkipped?.Invoke();
        }

        private void NotifyRewardedGranted()
        {
            OnRewardedGranted?.Invoke();
        }
    }
}