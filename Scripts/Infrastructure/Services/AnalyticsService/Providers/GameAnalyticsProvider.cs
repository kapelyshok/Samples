using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VInspector;
#if GAMEANALYTICS_SDK && !(UNITY_EDITOR)
using GameAnalyticsSDK;
#endif

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public class GameAnalyticsProvider : BaseAnalyticsProvider
    {
        public override event Action<Dictionary<string, object>> OnEventSent;

        public override async UniTask SendEvent(string key, Dictionary<string, object> data = null)
        {
#if GAMEANALYTICS_SDK && !(UNITY_EDITOR)
            if (data != default)
            {
                GameAnalytics.NewDesignEvent(key, data);
            }
            else
            {
                GameAnalytics.NewDesignEvent(key);
            }
#endif
            OnEventSent?.Invoke(data);
        }

        public override async UniTask Initialize()
        {
#if GAMEANALYTICS_SDK && !(UNITY_EDITOR)
            GameAnalytics.Initialize();
#endif
        }

        private void OnDestroy()
        {
            UnsubscribeMaxSDKAdRevenue();
        }

        public override void SubscribeMaxSDKAdRevenue()
        {
#if GAMEANALYTICS_SDK && APPLOVIN_SDK && !(UNITY_EDITOR)
            if (IsCollectingAdRevenue)
            {
                GameAnalyticsILRD.SubscribeMaxImpressions();
            }
#endif
        }

        private void UnsubscribeMaxSDKAdRevenue()
        {
            //GameAnalytics should automatically unsubscribe when needed
        }
    }
}