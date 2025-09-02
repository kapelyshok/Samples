using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public abstract class BaseAnalyticsProvider : MonoBehaviour
    {
        public bool IsCollectingAdRevenue;
        public abstract UniTask SendEvent(string key, Dictionary<string, object> data = null);
        public abstract UniTask Initialize();
        public abstract void SubscribeMaxSDKAdRevenue();
        public abstract event Action<Dictionary<string, object>> OnEventSent;
    }
}