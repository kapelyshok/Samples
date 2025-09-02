using System;
using System.Collections.Generic;
using AtomicApps.Infrastructure.Services.SaveLoad.Data;
using UnityEngine;

namespace AtomicApps.Infrastructure.Currencies
{
    public class CurrencyWallet
    {
        private CurrencyType _currencyType;
        private int _amount;

        public CurrencyType CurrencyType => _currencyType;
        public event Action<CurrencyWallet,bool> OnWalletUpdated;

        public CurrencyWallet(CurrencyType currencyType)
        {
            _currencyType = currencyType;
        }

        public void SetAmount(int amount, bool animateCounters = true)
        {
            _amount = amount;
            OnWalletUpdated?.Invoke(this, animateCounters);
        }

        public int GetAmount()
        {
            return _amount;
        }

        public float AddAmount(int value, bool animateCounters = true)
        {
            _amount += value;
            OnWalletUpdated?.Invoke(this, animateCounters);
            return _amount;
        }
    }
}