using System.Collections.Generic;
using System.Threading;
using AtomicApps.Infrastructure.Services.AdsService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps.Sdk
{
    public class AdsRewardButton : MonoBehaviour
    {
        [SerializeField] private Button rewardButton;
        [SerializeField] private AdsPlacement adsPlacement;
        
        [SerializeField] private UnityEvent receiveRewardEvent;
        [SerializeField] private UnityEvent skippedRewardEvent;

        private GameObject _loadingImage;
        private CancellationTokenSource _internetCheckCts;
        private IAdsService _adsService;
        //private IAnalyticsService _analyticsService;

        [Inject]
        private void Construct(IAdsService adsService/*, IAnalyticsService analyticsService*/)
        {
            //_analyticsService = analyticsService;
            _adsService = adsService;
        }

        private void Start()
        {
            _adsService.OnRewardedGranted += OnRewardedAdReceivedReward;
            _adsService.OnRewardedSkipped += OnRewardedAdSkippedReward;
            rewardButton.onClick.AddListener(RequestRewardedAd);
        }

        private void OnDestroy()
        {
            _adsService.OnRewardedGranted -= OnRewardedAdReceivedReward;
            _adsService.OnRewardedSkipped -= OnRewardedAdSkippedReward;
            rewardButton.onClick.RemoveListener(RequestRewardedAd);
        }

        private void RequestRewardedAd()
        {
            _adsService.ShowRewarded();
            //_analyticsService.AdsStarted(AdsType.rewarded, adsPlacement);
        }

        private void OnRewardedAdReceivedReward()
        {
            //_analyticsService.AdsWatched(AdsType.rewarded, adsPlacement);
            receiveRewardEvent?.Invoke();
        }

        private void OnRewardedAdSkippedReward()
        {
            skippedRewardEvent?.Invoke();
        }
    }
}