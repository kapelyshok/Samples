//using AppsFlyerSDK;
//using Facebook.Unity;
//using Io.AppMetrica;

using System.Collections.Generic;
using AtomicApps.HCUnavinarCore;
using AtomicApps.Infrastructure.Services.AdsService;
using AtomicApps.Infrastructure.Services.SaveLoad;
using AtomicApps.Sdk;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.Services.AnalyticsService
{
    public class AnalyticsService : MonoBehaviour//, IAnalyticsService
    {
        [SerializeField]
        private List<BaseAnalyticsProvider> analyticsProviders; 

        private SavableValue<int> _rewardAdsWatched;
        private SavableValue<List<RewardPlacementData>> _adsRewardLoadedData;
        private SavableValue<List<RewardPlacementData>> _adsRewardShownData;

        private readonly HashSet<int> _rewardAdsMilestones = new HashSet<int> { 5, 10, 20, 30, 50, 100 };
        
        private readonly HashSet<AdsPlacement> _rewardAdsAdsPlacements = new HashSet<AdsPlacement>
        {

        };
        
        private ISaveService _saveService;

        [Inject]
        private void Construct(ISaveService saveService)
        {
            _saveService = saveService;
        }

        protected void Awake()
        {
            _rewardAdsWatched = new SavableValue<int>(AnalyticsKeys.ADS_WATCHED, 0);
            _adsRewardLoadedData = new SavableValue<List<RewardPlacementData>>(AnalyticsKeys.ADS_REWARD_LOADED, new List<RewardPlacementData>());
            _adsRewardShownData = new SavableValue<List<RewardPlacementData>>(AnalyticsKeys.ADS_REWARD_SHOWN, new List<RewardPlacementData>());

            InitRewardData(_adsRewardLoadedData);
            InitRewardData(_adsRewardShownData);
        }

        private void InitRewardData(SavableValue<List<RewardPlacementData>> rewardPlacementData)
        {      
            gameObject.SetActive(true);            
            if (rewardPlacementData.Value.Count == 0)
            {
                foreach (var AdsPlacement in _rewardAdsAdsPlacements)
                    rewardPlacementData.Value.Add(new RewardPlacementData(AdsPlacement, 0));
            }
        }

        /*public void Init()
        {
            if (string.IsNullOrEmpty(devKeyAppFly)) return;

            AppsFlyer.initSDK(devKeyAppFly, "");
            AppsFlyer.startSDK();

            AppMetrica.Activate(new AppMetricaConfig(appMetricaKey)
            {
                Logs = true,
                DataSendingEnabled = true,
                FirstActivationAsUpdate = !_firstEntry.Value,
                SessionTimeout = 300
            });

            SendStart();
        }

        private void SendStart()
        {
            if (FirstEntry)
            {
                _firstEntry.Value = false;
                FirstAppLaunch();

                return;
            }

            AppLaunch();
        }

        private void FirstAppLaunch()
        {
            AppMetrica.ReportEvent("first_app_launch");
            AppMetrica.SendEventsBuffer();

            Debug.Log("First app launch");
        }

        private void AppLaunch()
        {
            AppMetrica.ReportEvent("app_launch");
            AppMetrica.SendEventsBuffer();

            Debug.Log("App launch");
        }

        public void IdleGetChest(int carIndex, int chestIndex)
        {
            var param = new Dictionary<string, object>
            {
                { $"chest_{carIndex}", chestIndex},
            };
            
            AppMetrica.ReportEvent("idle_get_chest", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();
        }
        
        public void CarTuningBought(int carIndex, int tuningIndex)
        {
            var param = new Dictionary<string, object>
            {
                { $"tuning_{carIndex}", tuningIndex},
            };

            AppMetrica.ReportEvent("car_tuning_bought", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();
        }
        
        public void StartRacing(int carIndex, int raceIndex)
        {
            var param = new Dictionary<string, object>
            {
                { $"race_{carIndex}", raceIndex },
            };

            AppMetrica.ReportEvent("start_racing", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();
        }
        
        public void CompleteRacing(int carIndex, int raceIndex)
        {
            var param = new Dictionary<string, object>
            {
                { $"race_{carIndex}", raceIndex },
            };

            AppMetrica.ReportEvent("complete_racing", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();
        }

        public void TutorialStartStep(int tutorialIndex, int step)
        {
            Debug.Log($"tutorial{tutorialIndex} step {step}");
            
            var param = new Dictionary<string, object>
            {
                { $"tutorial_{tutorialIndex}", step },
            };

            AppMetrica.ReportEvent("start_step_tutorial", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();
        }


        public void TutorialCompleteStep(string blockName, int step)
        {
            var param = new Dictionary<string, object>
            {
                { "step_number", step },
            };

            AppMetrica.ReportEvent($"tutorial_complete_{blockName}", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();

            Debug.Log($"Tutorial complete {blockName} step {step}");
        }

        public void ConnectionLost(int counter)
        {
            var param = new Dictionary<string, object>
            {
                { $"connection_lost", counter },
            };

            AppMetrica.ReportEvent("connection_lost", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();

            Debug.Log($"Connection lost {counter} times");
        }

        public void AdsStarted(AdsType type, AdsPlacement AdsPlacement)
        {
            var param = new Dictionary<string, object>
            {
                { "ad_type", type.ToString() },
                { "placement", AdsPlacement.ToString() },
                { "result", "start" }
            };

            AppMetrica.ReportEvent("video_ads_started", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();

            AdsStarted(AdsPlacement);
        }

        public void AdsAvailable(AdsType type, AdsPlacement AdsPlacement, bool IsSuccess)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            Dictionary<string, string> paramAFly = new Dictionary<string, string>();

            if (IsSuccess)
            {
                param = new Dictionary<string, object>
                {
                    { "ad_type", type.ToString() },
                    { "placement", AdsPlacement.ToString() },
                    { "result", "success" }
                };
            }
            else
            {
                param = new Dictionary<string, object>
                {
                    { "ad_type", type.ToString() },
                    { "placement", AdsPlacement.ToString() },
                    { "result", "not_available" }
                };
            }

            AppMetrica.ReportEvent("video_ads_available", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();
        }

        public void AdsWatched(AdsType type, AdsPlacement AdsPlacement)
        {
            var param = new Dictionary<string, object>
            {
                { "ad_type", type.ToString() },
                { "placement", AdsPlacement.ToString() },
                { "result", "watched" }
            };

            AppMetrica.ReportEvent("video_ads_watched", JsonConvert.SerializeObject(param));
            AppMetrica.SendEventsBuffer();

            AdsWatched(AdsPlacement);
            UserWatchAds();
        }

        private void AdsStarted(AdsPlacement AdsPlacement)
        {
            foreach (var placementData in _adsRewardLoadedData.Value)
            {
                if (placementData.AdsPlacement == AdsPlacement)
                {
                    placementData.IncreaseAmount();
                    _adsRewardLoadedData.SaveToPrefs();

                    var param = new Dictionary<string, object>
                    {
                        { $"{StringHelper.GetFormattedString(placementData.AdsPlacement)}", placementData.Amount },
                    };

                    AppMetrica.ReportEvent("ad_rewarded_loaded", JsonConvert.SerializeObject(param));
                    AppMetrica.SendEventsBuffer();

                    Debug.Log($"User tap on placement {placementData.AdsPlacement} {placementData.Amount} times");

                    break;
                }
            }
        }

        private void AdsWatched(AdsPlacement AdsPlacement)
        {
            foreach (var placementData in _adsRewardShownData.Value)
            {
                if (placementData.AdsPlacement == AdsPlacement)
                {
                    placementData.IncreaseAmount();
                    _adsRewardShownData.SaveToPrefs();

                    var param = new Dictionary<string, object>
                    {
                        { $"{StringHelper.GetFormattedString(placementData.AdsPlacement)}", placementData.Amount },
                    };

                    AppMetrica.ReportEvent("ad_rewarded_shown", JsonConvert.SerializeObject(param));
                    AppMetrica.SendEventsBuffer();

                    Debug.Log($"User took reward {placementData.AdsPlacement} {placementData.Amount} times");

                    break;
                }
            }
        }

        private void UserWatchAds()
        {
            _rewardAdsWatched.Value++;

            if (!_rewardAdsMilestones.Contains(_rewardAdsWatched.Value))
                return;

            AppMetrica.ReportEvent($"{_rewardAdsWatched.Value}th_rewarded");
            AppMetrica.SendEventsBuffer();

            Debug.Log($"The user has watched its {_rewardAdsWatched.Value}th rewarded video ");
        }*/
    }
}
