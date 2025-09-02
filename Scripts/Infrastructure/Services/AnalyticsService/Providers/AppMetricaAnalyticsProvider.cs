using System;
using System.Collections.Generic;
using AtomicApps.Infrastructure.Services.SaveLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VInspector;
using Zenject;

#if APPMETRICA_SDK && !(UNITY_EDITOR)
using Io.AppMetrica;
#endif

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public class AppMetricaAnalyticsProvider : BaseAnalyticsProvider
    {
#if APPMETRICA_SDK
        [SerializeField] private string key;
#endif
        
        private ISaveService _saveService;

        public override event Action<Dictionary<string, object>> OnEventSent;

        [Inject]
        private void Construct(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public override async UniTask SendEvent(string key, Dictionary<string, object> data = null)
        {
#if APPMETRICA_SDK && !(UNITY_EDITOR)
            if(data != default)
            {
                var json = JsonConvert.SerializeObject(param);
                AppMetrica.ReportEvent(key, json);
            }
            else
            {
                AppMetrica.ReportEvent(key);
            }

            AppMetrica.SendEventsBuffer();
#endif
            OnEventSent?.Invoke(data);
        }

        public override async UniTask Initialize()
        {
#if APPMETRICA_SDK && !(UNITY_EDITOR)
            AppMetrica.Activate(new AppMetricaConfig(key)
            {
                Logs = true,
                DataSendingEnabled = true,
                FirstActivationAsUpdate = !_saveService.GetData<SystemGeneralSaveData>().IsFirstLaunch,
                SessionTimeout = 300
            });
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
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAppMetricaAdRevenuePaidEvent;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAppMetricaAdRevenuePaidEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAppMetricaAdRevenuePaidEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAppMetricaAdRevenuePaidEvent;
            }
#endif
        }

        private void UnsubscribeMaxSDKAdRevenue()
        {
#if APPLOVIN_SDK && !(UNITY_EDITOR)
            if (IsCollectingAdRevenue)
            {
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnAppMetricaAdRevenuePaidEvent;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnAppMetricaAdRevenuePaidEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnAppMetricaAdRevenuePaidEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent -= OnAppMetricaAdRevenuePaidEvent;
            }
#endif
        }

#if APPLOVIN_SDK && !(UNITY_EDITOR)
        private void OnAppMetricaAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo adInfo)
        {
#if APPMETRICA_SDK
            AdRevenue adRevenue = new AdRevenue(adInfo.Revenue, "USD");
            adRevenue.AdNetwork = adInfo.NetworkName;
            adRevenue.AdPlacementId = adInfo.Placement;
            adRevenue.AdType = ConvertAdFormatToAdType(adInfo.AdFormat);
            adRevenue.AdUnitId = adInfo.AdUnitIdentifier;
            
            AppMetrica.ReportAdRevenue(adRevenue);
#endif
        }
#endif
        
#if APPMETRICA_SDK && !(UNITY_EDITOR)
        private AdType ConvertAdFormatToAdType(string adFormat)
        {
            switch (adFormat)
            {
                case "banner":
                    return AdType.Banner;
                case "interstitial":
                    return AdType.Interstitial;
                case "rewarded_video":
                    return AdType.Rewarded;
                case "mrec":
                    return AdType.Mrec;
                default:
                    return AdType.Other;
            }
        }
#endif
    }
}