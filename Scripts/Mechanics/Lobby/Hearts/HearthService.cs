using System;
using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Infrastructure.Services.SaveLoad;
using AtomicApps.Mechanics.Lobby.Hearts;
using UnityEngine;
using Zenject;

namespace AtomicApps.Scpts.Mechanics.Lobby.Hearts
{
    public class HearthService : MonoBehaviour, IHearthService
    {
        public int Max { get; private set; }
        public int Count => (int)_currencyWallet.GetAmount();
        public bool HasHeartsToPlay => IsFree || Count > 0;
        public float FreeTime => _freeHealthTimer.CurrentValue;
        public bool IsFree => _freeHealthTimer.CurrentValue > 0;
        public bool IsMax => Count >= Max;

        public event Action OnUpdated;
        
        private IHeartRegenService _heartRegenService;
        private CurrencyWallet _currencyWallet;
        private ISaveService _saveService;
        private GameConfigSO _gameConfigSo;
        private HeartSaveData _saveData;
        private ICurrenciesService _currenciesService;
        private BetterTimer _freeHealthTimer = new();

        [Inject]
        private void Construct(IHeartRegenService heartRegenService, GameConfigSO gameConfigSo, ISaveService saveService, ICurrenciesService currenciesService)
        {
            _currenciesService = currenciesService;
            _gameConfigSo = gameConfigSo;
            _saveService = saveService;
            _heartRegenService = heartRegenService;
            
            Max = gameConfigSo.MaxHearts;
        }

        public void Awake()
        {
            _saveData = _saveService.GetData<HeartSaveData>(new HeartSaveData()
            {
                LastCloseTime = DateTime.Now, 
                FreeHealthLeftTime = 0, 
                RemainingTimeForNextHeart = _gameConfigSo.HeartCooldownSeconds
            });
            
            _currencyWallet = _currenciesService.GetCurrencyWallet(CurrencyType.HEARTS);
            
            if (_freeHealthTimer.CurrentValue > 0)
            {
                _freeHealthTimer.Reset();
            }
            
            _heartRegenService.OnIncreameantHearts += OnIncrementLifeHandler;
            
            _currencyWallet.OnWalletUpdated += OnChangedHandler;
            
            _heartRegenService.Initialize();
        }

        private void OnDestroy()
        {
            _saveData.FreeHealthLeftTime = _freeHealthTimer.CurrentValue;
            _heartRegenService.OnIncreameantHearts -= OnIncrementLifeHandler;

            _saveService.SaveDataImmediately(_saveData);
        }

        public void SetFreeFor(int time)
        {
            _freeHealthTimer.SetTime(_freeHealthTimer.CurrentValue + time);
            _freeHealthTimer.Reset();
        }
        
        public void SpendHearth()
        {
            if(IsFree) return;
            _currencyWallet.AddAmount(-1);
            
            if (!IsMax)
                _heartRegenService.Resume();
        }

        public bool TryAddHearth()
        {
            if (IsMax)
            {
                return false;
            }

            _currencyWallet.AddAmount(1);

            if (IsMax)
            {
                _heartRegenService.ResetTimer();
                _heartRegenService.Stop();
            }
            
            return true;
        }

        private void OnIncrementLifeHandler(int add)
        {
            for(int  i = 0; i < add; i++)
            {
                TryAddHearth();
            }
        }
        
        private void OnChangedHandler(CurrencyWallet wallet, bool animateCounters)
        {
            OnUpdated?.Invoke();
        }

        public void AddMax()
        {
            while (!IsMax)
            {
                TryAddHearth();
            }
        }

        public void Tick()
        {
            _freeHealthTimer.Tick();
        }
    }
    
}
