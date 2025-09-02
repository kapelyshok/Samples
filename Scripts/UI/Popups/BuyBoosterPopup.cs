using System;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Infrastructure.Services.AdsService;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers.Boosters;
using AtomicApps.UI.Mechanics;
using TMPro;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class BuyBoosterPopup : BasePopup
    {
        [SerializeField]
        private List<BoosterImageData> boosterImages;
        [SerializeField]
        private CustomButton buyButton;
        [SerializeField]
        private CustomAdsButton adsButton;
        [SerializeField]
        private TextMeshProUGUI priceText;
        [SerializeField]
        private TextMeshProUGUI countText;
        [SerializeField]
        private TextMeshProUGUI countTextAds;
        
        [SerializeField]
        private Color defaultPriceColor;
        [SerializeField]
        private Color notEnoughPriceColor;
        
        private BoosterType _boosterType;
        private int _boosterPrice;
        private GameConfigSO _gameConfigSo;
        private ICurrenciesService _currenciesService;
        private IAdsService _adsService;
        private IAudioService _audioService;

        [Inject]
        private void Construct(GameConfigSO gameConfigSO, ICurrenciesService currenciesService, IAdsService adsService,
            IAudioService audioService)
        {
            _audioService = audioService;
            _adsService = adsService;
            _currenciesService = currenciesService;
            _gameConfigSo = gameConfigSO;
        }
        
        public override void Show(object[] inData = null)
        {
            if (inData != null && inData.Length > 0)
            {
                if (inData[0] is BoosterType)
                {
                    _boosterType = (BoosterType)inData[0];
                    
                    foreach (var boosterImage in boosterImages)
                    {
                        boosterImage.Image.SetActive(boosterImage.BoosterType == _boosterType);
                    }
                    
                    var price = _gameConfigSo.BoosterPricesConfig.BoosterPrices.FirstOrDefault(data=>data.BoosterType == _boosterType &&
                        data.CurrencyType == CurrencyType.COINS);
                    
                    if (price != null)
                    {
                        _boosterPrice = price.Cost;
                        priceText.text = price.Cost.ToString();
                        countText.text = price.Amount.ToString();
                        countTextAds.text = price.Amount.ToString();

                        if (_currenciesService.GetCurrencyWallet(CurrencyType.COINS).GetAmount() >= _boosterPrice)
                        {
                            priceText.color = defaultPriceColor;
                        }
                        else
                        {
                            priceText.color = notEnoughPriceColor;
                        }
                    }
                }
            }

            buyButton.OnClicked += TryBuyBooster;
            adsButton.OnRewardGranted += AddBooster;
            
            base.Show(inData);
        }

        private void TryBuyBooster()
        {
            _audioService.PlaySound(SoundKeys.TAP_OPEN);

            if (_currenciesService.GetCurrencyWallet(CurrencyType.COINS).GetAmount() >= _boosterPrice)
            {
                _currenciesService.GetCurrencyWallet(CurrencyType.COINS).AddAmount(-_boosterPrice);
                AddBooster();
            }
        }

        private void AddBooster()
        {
            _audioService.PlaySound(SoundKeys.SUCCESSFULL_BOUGHT);

            _adsService.OnRewardedGranted -= AddBooster;
            
            if (!Enum.TryParse(_boosterType.ToString(), ignoreCase: true, out CurrencyType boosterCurrency))
            {
                Debug.LogError($"No matching CurrencyType for BoosterType '{_boosterType}'.");
                Close();
                return;
            }
                
            _currenciesService.GetCurrencyWallet(boosterCurrency).AddAmount(1);
            
            Close();
        }
        

        public override void Close()
        {
            buyButton.OnClicked -= TryBuyBooster;
            adsButton.OnClicked -= AddBooster;
            
            base.Close();
        }
    }
    
    
    [Serializable]
    internal class BoosterImageData
    {
        public BoosterType BoosterType;
        public GameObject Image;
    }
}