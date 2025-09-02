using System;
using System.Collections;
using System.Collections.Generic;
using AtomicApps.Infrastructure.Currencies;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps
{
    public class CurrencyCounterView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI currencyAmountText;
        [SerializeField]
        private CurrencyType currencyType;
        [SerializeField]
        private Image image;
        
        private ICurrenciesService _currencyService;
        private CurrencyWallet _wallet;
        private int _currentValue;
        private bool _isUpdateBlocked;
        
        public Image Image => image;

        [Inject]
        private void Construct(ICurrenciesService currencyService)
        {
            _currencyService = currencyService;
            
            _wallet = _currencyService.GetCurrencyWallet(currencyType);
            UpdateText(_wallet, false);
            _currentValue = _currencyService.GetCurrencyWallet(currencyType).GetAmount();
            
            _wallet.OnWalletUpdated += WalletUpdatedHandler;
        }

        public void ChangeBlockUpdating(bool state)
        {
            _isUpdateBlocked = state;

            if (state == false)
            {
                UpdateText(_currencyService.GetCurrencyWallet(currencyType));
            }
        }

        private void OnDestroy()
        {
            _wallet.OnWalletUpdated -= WalletUpdatedHandler;
        }

        private void WalletUpdatedHandler(CurrencyWallet wallet, bool animateCounters)
        {
            UpdateText(wallet, animateCounters);
        }
        
        private void UpdateText(CurrencyWallet wallet, bool isAnimate = true)
        {
            if(_isUpdateBlocked) return;
            
            int startValue = _currentValue;
            int endValue = wallet.GetAmount();

            if (isAnimate)
            {
                DOTween.To(
                    () => startValue,
                    x =>
                    {
                        startValue = x;
                        currencyAmountText.text = startValue.ToString();
                    },
                    endValue,
                    .5f
                ).OnComplete(() =>
                {
                    currencyAmountText.text = wallet.GetAmount().ToString();
                    _currentValue = wallet.GetAmount();
                });
            }
            else
            {
                currencyAmountText.text = wallet.GetAmount().ToString();
                _currentValue = wallet.GetAmount();
            }
        }
    }
}
