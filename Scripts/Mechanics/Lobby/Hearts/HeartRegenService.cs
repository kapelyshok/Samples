using System;
using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Infrastructure.Services.SaveLoad;
using AtomicApps.Scpts.Mechanics.Lobby.Hearts;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Lobby.Hearts
{
    public class HeartRegenService : MonoBehaviour, IHeartRegenService
    {
        private int _replenishIntervalInSeconds;

        public string LeftTimeToString => _betterTimer.IsPaused || _betterTimer.IsCompleted ? string.Empty :
            GetLeftTimeToString();

        private DateTime _lastCloseDate;
        private float _remainingTimeForNextHeartLoad = float.MaxValue;

        public event Action<int> OnIncreameantHearts;

        private BetterTimer _betterTimer = new();
        private ISaveService _saveService;
        private GameConfigSO _gameConfigSo;
        private HeartSaveData _saveData;
        private ICurrenciesService _currenciesService;

        [Inject]
        public void Construct(ISaveService saveService, GameConfigSO gameConfigSo, ICurrenciesService currenciesService)
        {
            _currenciesService = currenciesService;
            _gameConfigSo = gameConfigSo;
            _saveService = saveService;
            _replenishIntervalInSeconds = gameConfigSo.HeartCooldownSeconds;
        }

        public void Initialize()
        {
            _saveData = _saveService.GetData<HeartSaveData>(new HeartSaveData()
            {
                LastCloseTime = DateTime.Now, 
                FreeHealthLeftTime = 0, 
                RemainingTimeForNextHeart = _gameConfigSo.HeartCooldownSeconds
            });
            
            _lastCloseDate = _saveData.LastCloseTime;
            _remainingTimeForNextHeartLoad = _saveData.RemainingTimeForNextHeart;
            
            var hearts = GivenBonusForTimeCalculation.Calculate(_lastCloseDate, _remainingTimeForNextHeartLoad,
                _replenishIntervalInSeconds,
                out float remainingTimeForNextHeart);

            if (_currenciesService.GetCurrencyWallet(CurrencyType.HEARTS).GetAmount() + hearts >=
                _gameConfigSo.MaxHearts)
            {
                _betterTimer.SetTime(_replenishIntervalInSeconds);
                _betterTimer.Reset();
                _betterTimer.Pause();
            }
            else
            {
                _betterTimer.SetTime(remainingTimeForNextHeart);
                _betterTimer.Reset();
            }
            
            InvokeOnIncrementHearts(hearts);
            
            _betterTimer.Completed += OnTimerCompleted;
        }

        public void ResetTimer()
        {
            _betterTimer.SetTime(_replenishIntervalInSeconds);
        }

        public void Update()
        {
            _betterTimer.Tick();
        }

        public void Stop()
        {
            _betterTimer.Pause();
            _saveData.RemainingTimeForNextHeart = _replenishIntervalInSeconds;
        }

        public void Resume()
        {
            _betterTimer.UnPause();
        }

        private void OnDestroy()
        {
            _saveData.LastCloseTime = DateTime.Now;
            _saveData.RemainingTimeForNextHeart = _betterTimer.CurrentValue;
            _saveService.SaveDataImmediately(_saveData);
        }

        private void OnTimerCompleted()
        {
            _betterTimer.SetTime(_replenishIntervalInSeconds);
            _betterTimer.Reset();
            InvokeOnIncrementHearts(1);

            Debug.Log($"Remained seconds to next Heart {_betterTimer.CurrentValue}");
        }

        private void InvokeOnIncrementHearts(int hearts)
        {
            if(hearts < 1)
                return;
            
            OnIncreameantHearts?.Invoke(hearts);
            Debug.Log($"Added {hearts} hearts by timer");
        }
        
        private string GetLeftTimeToString()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_betterTimer.CurrentValue);

            return string.Format("{0:D2}:{1:D2}",
                        timeSpan.Minutes,
                        timeSpan.Seconds);
        }
    }
}
