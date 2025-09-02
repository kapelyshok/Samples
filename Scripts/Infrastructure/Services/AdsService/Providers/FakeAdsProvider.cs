using System;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public class FakeAdsProvider : BaseAdsProvider
    {
        [SerializeField]
        private FakeInterstitial fakeInterstitial;
        [SerializeField]
        private FakeRewarded fakeRewarded;
        [SerializeField]
        private FakeBanner fakeBanner;

        public override event Action OnRewardedGranted;

        public override event Action OnRewardedSkipped;

        public override void Initialize()
        {
            fakeRewarded.OnRewardedGranted += NotifyRewardedGranted;
            fakeRewarded.OnRewardedSkipped += NotifyRewardedSkipped;
        }

        private void OnDestroy()
        {
            fakeRewarded.OnRewardedGranted -= NotifyRewardedGranted;
            fakeRewarded.OnRewardedSkipped -= NotifyRewardedSkipped;
        }

        public override bool IsInterstitialReady()
        {
            return true;
        }

        public override bool ShowInterstitial()
        {
            fakeInterstitial.Show();
            return true;
        }

        public override bool IsRewardedReady()
        {
            return true;
        }

        public override bool ShowRewarded()
        {
            fakeRewarded.Show();
            return true;
        }

        public override void InitializeBanner()
        {
            fakeBanner.Show();
        }

        public override void DisableBanner()
        {
            fakeBanner.Hide();
        }

        public override float GetBannerHeight()
        {
            return fakeBanner.GetHeight();
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