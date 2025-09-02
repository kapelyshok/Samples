using System;
using UnityEngine;
using VInspector;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public class ApplovinProvider : BaseAdsProvider
    {
#if APPLOVIN_SDK
        private const float BANNER_OFFSET = 40f;
        
        [SerializeField]
        private bool showMediationDebugger = false;
        
        [SerializeField] private string interAd = "";
        [SerializeField] private string rewardedAd = "";
        [SerializeField] private string bannerAd = "";
#endif
        private int _interstitialLoadAttempt;
        private int _rewardedLoadAttempt;
        private bool _wasRewarded;

        public override event Action OnRewardedGranted;

        public override event Action OnRewardedSkipped;

        public override void Initialize()
        {
#if APPLOVIN_SDK && !(UNITY_EDITOR)
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                // Interstitial Ad Callbacks
                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
                MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

                // Rewarded Ad Callbacks
                MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
                MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
                MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
                MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;

                MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

                LoadInterstitial();
                LoadRewardedAd();
                
                if(showMediationDebugger)
                {
                    MaxSdk.ShowMediationDebugger();
                }
            };
            
            MaxSdk.SetHasUserConsent(true);
            //MaxSdk.SetIsAgeRestrictedUser(false);
            MaxSdk.SetDoNotSell(false);
            MaxSdk.InitializeSdk();
#endif
        }

        public override bool IsInterstitialReady()
        {
#if APPLOVIN_SDK
            return MaxSdk.IsInterstitialReady(interAd);
#else
            return false;
#endif
        }

        public override bool ShowInterstitial()
        {
#if APPLOVIN_SDK
            if (MaxSdk.IsInterstitialReady(interAd))
            {
                MaxSdk.ShowInterstitial(interAd);
                LoadInterstitial();
                return true;
            }
            else
            {
                return false;
            }
#else
            return false;
#endif
        }

        public override bool IsRewardedReady()
        {
#if APPLOVIN_SDK
            return MaxSdk.IsRewardedAdReady(rewardedAd);
#else
            return false;
#endif
        }

        public override bool ShowRewarded()
        {
#if APPLOVIN_SDK
            if (MaxSdk.IsRewardedAdReady(rewardedAd))
            {
                _wasRewarded = false;
                MaxSdk.ShowRewardedAd(rewardedAd);
                LoadRewardedAd();
                return true;
            }
            else
            {
                return false;
            }
#else
            return false;
#endif
        }

        public override void InitializeBanner()
        {
#if APPLOVIN_SDK
            MaxSdk.CreateBanner(bannerAd, MaxSdkBase.BannerPosition.BottomCenter);

            MaxSdk.SetBannerBackgroundColor(bannerAd, Color.clear);

            MaxSdk.ShowBanner(bannerAd);
#endif
        }

        public override void DisableBanner()
        {
#if APPLOVIN_SDK
            MaxSdk.HideBanner(bannerAd);
#endif
        }

        public override float GetBannerHeight()
        {
#if APPLOVIN_SDK
            return MaxSdkUtils.GetAdaptiveBannerHeight() + BANNER_OFFSET;
#else
            return 0;
#endif
        }

#if APPLOVIN_SDK
        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(interAd);
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(rewardedAd);
        }
        
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
            // Reset retry attempt
            _interstitialLoadAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            _interstitialLoadAttempt++;
            double retryDelay = Mathf.Pow(2, Mathf.Min(6, _interstitialLoadAttempt));

            Invoke(nameof(LoadInterstitial), (float)retryDelay);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();
        }
        
        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

            // Reset retry attempt
            _rewardedLoadAttempt = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

            _rewardedLoadAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedLoadAttempt));

            Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            if (!_wasRewarded)
            {
                OnRewardedSkipped?.Invoke();
            }
            else
            {
                OnRewardedGranted?.Invoke();
            }
            
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.

            _wasRewarded = true;
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.

        }
#endif
        
#if UNITY_EDITOR
        [Button]
        private void EnableApplovinDefineSymbol()
        {
            DefineSymbolsHelper.AddAppLovinSDKSymbol();
        }
        [Button]
        private void DisableApplovinDefineSymbol()
        {
            DefineSymbolsHelper.RemoveAppLovinSDKSymbol();
        }
#endif
    }
}
