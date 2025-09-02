using System;
using Cysharp.Threading.Tasks;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public interface IAdsService
    {
        public UniTask Init();
        public bool IsInterstitialReady();
        public void ShowInterstitial();
        public bool IsRewardedReady();
        public void ShowRewarded();
        public void EnableBanner();
        public void DisableBanner();
        public BannerStateData CurrentBannerState { get; }
        public event Action OnRewardedGranted;
        public event Action OnRewardedSkipped;
        public event Action<BannerStateData> OnBannerStateChanged;
    }
}