using System;
using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.UI.Mechanics;
using TMPro;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class OutOfLivesPopup : BasePopup
    {
        [SerializeField]
        private CustomButton tryRefillForCoinsButton;
        [SerializeField]
        private CustomAdsButton tryRefillForAdsButton;
        [SerializeField]
        private TextMeshProUGUI priceText;
        
        private GameConfigSO _config;
        private ICurrenciesService _currenciesService;

        [Inject]
        private void Construct(GameConfigSO config, ICurrenciesService currenciesService)
        {
            _currenciesService = currenciesService;
            _config = config;
            priceText.text = _config.RefillHeartsCoinsPrice.ToString();
        }

        public override void Show(object[] inData = null)
        {
            tryRefillForCoinsButton.OnClicked += TryRefillForCoins;
            tryRefillForAdsButton.OnRewardGranted += GrantAdsReward;
            base.Show(inData);
        }

        public override void Close()
        {
            tryRefillForCoinsButton.OnClicked -= TryRefillForCoins;
            tryRefillForAdsButton.OnRewardGranted -= GrantAdsReward;
            base.Close();
        }

        private void TryRefillForCoins()
        {
            if (_currenciesService.GetCurrencyWallet(CurrencyType.COINS).GetAmount() >= _config.RefillHeartsCoinsPrice)
            {
                _currenciesService.GetCurrencyWallet(CurrencyType.COINS).AddAmount(-_config.RefillHeartsCoinsPrice);
                _currenciesService.GetCurrencyWallet(CurrencyType.HEARTS).AddAmount(_config.MaxHearts);
                Close();
            }
        }

        private void GrantAdsReward()
        {
            _currenciesService.GetCurrencyWallet(CurrencyType.HEARTS).AddAmount(1);
            Close();
        }
    }
}