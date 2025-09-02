using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VInspector;
#if APPSFLYER_SDK && !(UNITY_EDITOR)
using AppsFlyerSDK;
#endif

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public class AppsFlyerAnalyticsProvider : BaseAnalyticsProvider
    {
#if APPSFLYER_SDK
        [Header("Android")]
        [SerializeField] private string devKey;
        [Header("iOS")]
        [SerializeField] private string appID;
#endif
        public override event Action<Dictionary<string, object>> OnEventSent;

        public override async UniTask SendEvent(string key, Dictionary<string, object> data = null)
        {
#if APPSFLYER_SDK && !(UNITY_EDITOR)
            if(data != default)
            {
                var param = new Dictionary<string, string>();
                foreach (var kvp in data)
                {
                    param.Add(kvp.Key, kvp.Value.ToString());
                }

                AppsFlyer.sendEvent(key, param);
            }
            else
            {
                AppsFlyer.sendEvent(key, null);
            }
#endif
            OnEventSent?.Invoke(data);
        }

        public override async UniTask Initialize()
        {
#if APPSFLYER_SDK && !(UNITY_EDITOR)
            AppsFlyer.initSDK(devKey, appID);
            AppsFlyer.startSDK();
#endif
        }

        private void OnDestroy()
        {
            UnsubscribeMaxSDKAdRevenue();
        }

        public override void SubscribeMaxSDKAdRevenue()
        {
#if APPLOVIN_SDK && !(UNITY_EDITOR)
            if(IsCollectingAdRevenue)
            {
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnMaxSDKAdRevenuePaidEvent; 
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnMaxSDKAdRevenuePaidEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnMaxSDKAdRevenuePaidEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMaxSDKAdRevenuePaidEvent;
            }
#endif
        }

        private void UnsubscribeMaxSDKAdRevenue()
        {
#if APPLOVIN_SDK && !(UNITY_EDITOR)
            if(IsCollectingAdRevenue)
            {
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnMaxSDKAdRevenuePaidEvent;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnMaxSDKAdRevenuePaidEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnMaxSDKAdRevenuePaidEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent -= OnMaxSDKAdRevenuePaidEvent;
            }
#endif
        }

#if APPLOVIN_SDK && !(UNITY_EDITOR)
        private void OnMaxSDKAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
#if APPSFLYER_SDK
            Dictionary<string, string> additionalParams = new Dictionary<string, string>();
            additionalParams.Add(AdRevenueScheme.COUNTRY, MaxSdk.GetSdkConfiguration().CountryCode);
            additionalParams.Add(AdRevenueScheme.AD_UNIT, adInfo.AdUnitIdentifier);
            additionalParams.Add(AdRevenueScheme.AD_TYPE, adInfo.AdFormat);
            additionalParams.Add(AdRevenueScheme.PLACEMENT, adInfo.Placement);
            var logRevenue = new AFAdRevenueData(adInfo.NetworkName, MediationNetwork.ApplovinMax, "USD", adInfo.Revenue);
            AppsFlyer.logAdRevenue(logRevenue, additionalParams);
#endif
        }
#endif
    }
}