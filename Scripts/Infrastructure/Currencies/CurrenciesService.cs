using System;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Services.SaveLoad;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.Currencies
{
    public class CurrenciesService : MonoBehaviour, ICurrenciesService
    {
        private Dictionary<CurrencyType, CurrencyWallet> _wallets = new Dictionary<CurrencyType, CurrencyWallet>();
        
        private CurrenciesConfigSO _config;
        private CurrenciesSaveData _saveData;
        private ISaveService _saveService;

        [Inject]
        private void Construct(ISaveService saveService, GameConfigSO config)
        {
            _saveService = saveService;
            _config = config.CurrenciesConfig;
        }

        public CurrencyWallet GetCurrencyWallet(CurrencyType currencyType)
        {
            return _wallets[currencyType];
        }

        private void Awake()
        {
            var defaultSaveData = PrepareDefaultSaveData();
            
            _saveData = _saveService.GetData<CurrenciesSaveData>(defaultSaveData);
            
            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                _wallets[currency] = new CurrencyWallet(currency);
            }
            
            foreach (var wallet in _wallets)
            {
                var amount = _saveData.CurrenciesData.FirstOrDefault(currency=>
                    currency.Currency == wallet.Value.CurrencyType);
                
                if (amount != null)
                {
                    wallet.Value.SetAmount(amount.Value);
                }

                wallet.Value.OnWalletUpdated += UpdateSaveData;
            }
        }

        private void OnDestroy()
        {
            foreach (var wallet in _wallets)
            {
                wallet.Value.OnWalletUpdated -= UpdateSaveData;
            }
        }

        private CurrenciesSaveData PrepareDefaultSaveData()
        {
            CurrenciesSaveData saveData = new CurrenciesSaveData();
            
            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                var dataFromConfig = _config.CurrenciesData.FirstOrDefault(x => x.Currency == currency);
                if (dataFromConfig != null)
                {
                    saveData.CurrenciesData.Add(new CurrencyValueData(dataFromConfig));
                }
                else
                {
                    saveData.CurrenciesData.Add(new CurrencyValueData(){Currency = currency, Value = 0});
                }
            }
            
            return saveData;
        }

        private void UpdateSaveData(CurrencyWallet wallet, bool animateCounters)
        {
            var data = _saveData.GetCurrencyValue(wallet.CurrencyType);
            
            if (data != null)
            {
                data.Value = wallet.GetAmount();
            }
            else
            {
                _saveData.CurrenciesData.Add(new CurrencyValueData(){Currency = wallet.CurrencyType, Value = wallet.GetAmount()});
            }
            
            _saveService.SaveDataImmediately(_saveData);
        }
    }
}
