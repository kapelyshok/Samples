using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.Mechanics.Gameplay.Levels;
using AtomicApps.Scpts.Mechanics.Lobby.Hearts;
using AtomicApps.UI.Mechanics;
using TMPro;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class KeepPlayingPopup : BasePopup
    {
        [SerializeField]
        private TextMeshProUGUI submitsCountText;
        [SerializeField]
        private TextMeshProUGUI priceText;
        [SerializeField]
        private CustomButton buyButton;
        [SerializeField]
        private CustomAdsButton adsButton;
        [SerializeField]
        private CustomButton closeButton;
        [SerializeField]
        private Color normalPriceTextColor;
        [SerializeField]
        private Color notEnoughPriceTextColor;

        private IPopupService _popupService;
        private GameConfigSO _gameConfigSo;
        private ICurrenciesService _currenciesService;
        private IHearthService _hearthService;
        private LevelStagesManager _levelStagesManager;
        private IAudioService _audioService;

        [Inject]
        private void Construct(GameConfigSO gameConfigSo, ICurrenciesService currenciesService, 
            IPopupService popupService, IHearthService hearthService, LevelStagesManager levelStagesManager,
            IAudioService audioService)
        {
            _audioService = audioService;
            _levelStagesManager = levelStagesManager;
            _hearthService = hearthService;
            _currenciesService = currenciesService;
            _gameConfigSo = gameConfigSo;
            _popupService = popupService;
        }

        public override void Show(object[] inData = null)
        {
            submitsCountText.text = _gameConfigSo.KeepPlayingBonusSubmits.ToString();
            priceText.text = _gameConfigSo.KeepPlayingBonusSubmitsCoinPrice.ToString();
            var coins = _currenciesService.GetCurrencyWallet(CurrencyType.COINS).GetAmount();
            priceText.color = coins >= _gameConfigSo.KeepPlayingBonusSubmitsCoinPrice ? normalPriceTextColor : notEnoughPriceTextColor;

            buyButton.OnClicked += TryBuyKeepPlaying;
            adsButton.OnRewardGranted += GrantReward;
            closeButton.OnClicked += FailLevel;
            base.Show(inData);
        }

        private void FailLevel()
        {
            _hearthService.SpendHearth();
            _popupService.ShowPopupAsync(PopupKeys.LEVEL_FAILED_POPUP, UIConstants.PopupShow.ReplaceCurrent);
        }

        public override void Close()
        {
            _audioService.PlaySound(SoundKeys.TAP_CLOSE);

            buyButton.OnClicked -= TryBuyKeepPlaying;
            adsButton.OnRewardGranted -= GrantReward;
            base.Close();
        }

        private void TryBuyKeepPlaying()
        {
            var coins = _currenciesService.GetCurrencyWallet(CurrencyType.COINS).GetAmount();
            if (coins >= _gameConfigSo.KeepPlayingBonusSubmitsCoinPrice)
            {
                _currenciesService.GetCurrencyWallet(CurrencyType.COINS).AddAmount(-_gameConfigSo.KeepPlayingBonusSubmitsCoinPrice);
                GrantReward();
            }
        }

        private void GrantReward()
        {
            _levelStagesManager.ChangeWordAttemptsLeft(_gameConfigSo.KeepPlayingBonusSubmits);
            Close();
        }
    }
}