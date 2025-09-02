using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public abstract class BaseAdsProvider : MonoBehaviour
    {
        public abstract void Initialize();
        
        public abstract bool IsInterstitialReady();
        public abstract bool ShowInterstitial();
        
        public abstract bool IsRewardedReady();
        public abstract bool ShowRewarded();

        public abstract void InitializeBanner();
        public abstract void DisableBanner();
        public abstract float GetBannerHeight(); 
        
        public abstract event Action OnRewardedGranted;
        public abstract event Action OnRewardedSkipped;
    }
}
