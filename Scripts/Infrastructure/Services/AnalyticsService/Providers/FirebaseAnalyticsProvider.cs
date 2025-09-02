using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VInspector;
#if FIREBASE_ANALYTICS_SDK && !(UNITY_EDITOR)
using Firebase.Analytics;
#endif

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public class FirebaseAnalyticsProvider : BaseAnalyticsProvider
    {
        public override event Action<Dictionary<string, object>> OnEventSent;

        public override async UniTask SendEvent(string key, Dictionary<string, object> data = null)
        {
#if FIREBASE_ANALYTICS_SDK && !(UNITY_EDITOR)
            if (data != null)
            {
                List<Parameter> firebaseParameters = new List<Parameter>();
                foreach (var kvp in data)
                {
                    firebaseParameters.Add(new Parameter(kvp.Key, kvp.Value.ToString()));
                }

                FirebaseAnalytics.LogEvent(key, firebaseParameters.ToArray());
            }
            else
            {
                FirebaseAnalytics.LogEvent(key);
            }
#endif
            OnEventSent?.Invoke(data);
        }

        public override async UniTask Initialize()
        {
#if FIREBASE_ANALYTICS_SDK && !(UNITY_EDITOR)
            
#endif
        }

        private void OnDestroy()
        {
            UnsubscribeMaxSDKAdRevenue();
        }

        public override void SubscribeMaxSDKAdRevenue()
        {
#if APPLOVIN_SDK && !(UNITY_EDITOR)
            if (IsCollectingAdRevenue)
            {
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnFirebaseAdRevenuePaidEvent; 
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnFirebaseAdRevenuePaidEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnFirebaseAdRevenuePaidEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnFirebaseAdRevenuePaidEvent;
            }
#endif
        }

        private void UnsubscribeMaxSDKAdRevenue()
        {
#if APPLOVIN_SDK && !(UNITY_EDITOR)
            if (IsCollectingAdRevenue)
            {
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnFirebaseAdRevenuePaidEvent;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnFirebaseAdRevenuePaidEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnFirebaseAdRevenuePaidEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent -= OnFirebaseAdRevenuePaidEvent;
            }
#endif
        }

#if APPLOVIN_SDK && !(UNITY_EDITOR)
        private void OnFirebaseAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
#if FIREBASE_ANALYTICS_SDK
            double revenue = adInfo.Revenue;
            var impressionParameters = new[] {
                new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
                new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
                new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
                new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
                new Firebase.Analytics.Parameter("value", revenue),
                new Firebase.Analytics.Parameter("currency", "USD"),
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
#endif
        }
#endif
    }
}