using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Infrastructure.Services.AdsService;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Mechanics.Gameplay.Levels;
using AtomicApps.UI.Mechanics;
using AtomicApps.UI.Popups;
using Cysharp.Threading.Tasks;
using TMPro;
using Unavinar;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class LevelCompletePopup : BasePopup
    {
        [SerializeField] private RouletteManager rouletteManager;
        [SerializeField] private CustomButton claimButton;
        [SerializeField] private CustomAdsButton claimAdsButton;
        [SerializeField] private GameObject roulleteGo;
        [SerializeField] private ClaimItemsWithCircleAnimation coinsAnimation;
        [SerializeField] private TextMeshProUGUI defaultRewardText;
        [SerializeField] private TextMeshProUGUI increasedRewardText;
        
        [SerializeField] private int entryLevel = 4;
        [Space]
        [Header("CoinAnimation")]
        //[SerializeField] private CoinsAnimationData coinsAnimationData;
        [SerializeField] private RectTransform coinSpawnPoint;
        [SerializeField] private RectTransform coinSpawnPointAds;
        [SerializeField] private CurrencyCounterView counter;
        //[SerializeField] private UiFeatureUnlockSlider featureSlider;
        private IAudioService _audioService;
        private ILevelSelectorService _levelSelectorService;
        private GameStateMachine _stateMachine;
        private IAdsService _adsService;
        private LevelStagesManager _levelStagesManager;
        private ICurrenciesService _currenciesService;
        private int _defaultCoinsReward = 1;
        //private GameDesignFeaturesUnlocker _featuresUnlocker;
        
        public event Action OnRewardGet;

        [Inject]
        private void Construct(IAudioService audioService, ILevelSelectorService levelSelectorService,
            GameStateMachine stateMachine, LevelStagesManager levelStagesManager, ICurrenciesService currenciesService/*GameDesignFeaturesUnlocker featuresUnlocker)*/)
        {
            _currenciesService = currenciesService;
            _levelStagesManager = levelStagesManager;
            _levelSelectorService = levelSelectorService;
            _audioService = audioService;
            _stateMachine = stateMachine;

            /*_featuresUnlocker = featuresUnlocker;*/
            //UpdateNextFeatureSlider();
            
            Init();
        }

        public override void Show(object[] inData = null)
        {
            _audioService.PlaySound(SoundKeys.WINSCREEN_SHOWN);
            
            var rewards = _levelStagesManager.GetCurrentLevelData().Rewards;
            var coinsReward = rewards.FirstOrDefault(data => data.Currency == CurrencyType.COINS);
            
            if (coinsReward == null)
            {
                Debug.LogError("Coin reward not found for this level");
                _defaultCoinsReward = 1;
                defaultRewardText.text = _defaultCoinsReward.ToString();
                increasedRewardText.text = _defaultCoinsReward.ToString();
            }
            else
            {
                _defaultCoinsReward = coinsReward.Value;
                defaultRewardText.text = _defaultCoinsReward.ToString();
                increasedRewardText.text = _defaultCoinsReward.ToString();
            }

            base.Show(inData);
        }

        private void Update()
        {
            if (rouletteManager.IsRouletteActive)
            {
                increasedRewardText.text = (_defaultCoinsReward * rouletteManager.GetMultiplier()).ToString();
            }
        }

        private void Init()
        {
            bool isEntryLevel = _levelSelectorService.GetCurrentLevelIndex() <= entryLevel;
            
            /*for (int i = 0; i < claimButtons.Count; i++)
            {
                if (i != 1 || !isEntryLevel)
                {
                    float time = !isEntryLevel ? displayTime[i] : 0f;
                    StartCoroutine(DisplayButtonInTime(time, claimButtons[i]));
                    claimButtons[i].OnClicked += OnClickedHandler;
                }
                else
                {
                    claimButtons[i].gameObject.SetActive(false);
                }
            }*/

            if (isEntryLevel)
            {
                roulleteGo.SetActive(false);
                claimAdsButton.gameObject.SetActive(false);
            }
            else
            {
                rouletteManager.Run();
            }

            claimButton.OnClicked += GrantDefaultReward;
            claimAdsButton.OnRewardGranted += GrantIncreasedReward;
        }

        public override void Close()
        {
            claimButton.OnClicked -= GrantDefaultReward;

            base.Close();
        }

        private void GrantDefaultReward()
        {
            claimButton.ChangeInteractable(false);
            claimAdsButton.ChangeInteractable(false);
            
            counter.ChangeBlockUpdating(true);
            rouletteManager.Stop();

            var rewards = _levelStagesManager.GetCurrentLevelData().Rewards;
            
            foreach (var reward in rewards)
            {
                _currenciesService.GetCurrencyWallet(reward.Currency).AddAmount(reward.Value);
            }
            
            SpawnCoins(coinSpawnPoint, 10);
        }
        
        private void GrantIncreasedReward()
        {
            claimButton.ChangeInteractable(false);
            claimAdsButton.ChangeInteractable(false);
            
            counter.ChangeBlockUpdating(true);
            rouletteManager.Stop();

            var rewards = _levelStagesManager.GetCurrentLevelData().Rewards;
            
            foreach (var reward in rewards)
            {
                if (reward.Currency == CurrencyType.COINS)
                {
                    var multiplier = rouletteManager.GetMultiplier();
                    _currenciesService.GetCurrencyWallet(reward.Currency).AddAmount((int)(reward.Value * multiplier));
                }
                else
                {
                    _currenciesService.GetCurrencyWallet(reward.Currency).AddAmount(reward.Value);
                }
            }
            
            SpawnCoins(coinSpawnPointAds, 20);
        }

        private void UpdateNextFeatureSlider()
        {
            //_audioService.Play(SoundKeys.WIN);
            
            /*var feature = _featuresUnlocker.GetNextFeature();
            if (feature == null)
            {
                featureSlider.gameObject.SetActive(false);
            }
            else
            {
                featureSlider.gameObject.SetActive(true);
                featureSlider.SetIcon(feature.id.ToString());
                featureSlider.SetRange(_featuresUnlocker.GetProgress());
                featureSlider.SetProgress(_featuresUnlocker.GetProgressPercent());
            }*/
        }
        
        private void SpawnCoins(RectTransform spawnPoint, int amount)
        {
            //_audioService.Play(SoundKeys.COIN_GET);
            SpawnMovableIcons(spawnPoint, counter.Image.rectTransform, amount);
        }

        private void SpawnMovableIcons(RectTransform spawnPoint, RectTransform to, int amount)
        {
            coinsAnimation.SetAmount(amount);
            coinsAnimation.SetDestinationPoint(to);
            coinsAnimation.SetSpawnPoint(spawnPoint);

            coinsAnimation.OnFirstItemAnimationComplete += () =>
            {
                counter.ChangeBlockUpdating(false);
            };
            
            coinsAnimation.OnAnimationComplete += (items) =>
            {
                GoToLobby();
            };
            
            coinsAnimation.StartAnimation();
            
            /*for (int i = 0; i < coinsAnimationData.CoinsCount; i++)
            {
                RectTransform coin = Instantiate(coinsAnimationData.CoinPrefab, content.transform);
                coin.position = spawnPoint.position;

                Sequence sequenceCoin = DOTween.Sequence();

                Vector3 spawnPosition = new Vector3(UnityRandom.Range(-coinsAnimationData.Offset.x, coinsAnimationData.Offset.x),
                    UnityRandom.Range(-coinsAnimationData.Offset.y, coinsAnimationData.Offset.y));

                float time = Vector3.Distance(spawnPosition, to.position)
                    / coinsAnimationData.SpeedToCounter;

                sequenceCoin.Append(
                coin.DOMove(coin.position + spawnPosition, coinsAnimationData.MoveToPositionTime)
                ).Append(coin.DOMove(to.position, time)).OnComplete(() =>
                {
                    Destroy(coin.gameObject);
                });
            }*/
        }

        private async UniTask GoToLobby()
        {
            await UniTask.WaitForSeconds(1f);
            _stateMachine.Enter<LoadLobbyState, bool>(true);
            Close();
        }
    }
}
