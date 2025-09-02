using System;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public class FakeInterstitial : BaseFakeAds
    {
        [SerializeField]
        private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(Hide);
        }
    }
}
