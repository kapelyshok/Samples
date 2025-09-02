using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Infrastructure.Services.AdsService;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Mechanics.Gameplay.Bonuses;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using AtomicApps.UI.Mechanics;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class StageCompletedPopup : BasePopup, ITriggerInitiator
    {
        [SerializeField]
        private List<BonusItemToSelectView> items;
        [SerializeField]
        private TextMeshProUGUI additionalTriesText;
        [SerializeField]
        private TextMeshProUGUI rerollPriceText;
        [SerializeField]
        private RectTransform perksContainer;
        [SerializeField]
        private GameObject freeReRollsButtonState;
        [SerializeField]
        private TextMeshProUGUI freeReRollsCountText;
        [SerializeField]
        private CustomButton reRollButton;
        [SerializeField]
        private CustomButton reRollButtonAds;
        [SerializeField]
        private HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField]
        private List<PerkItemView> fakePerkViews;

        [Header("Out animation")]
        [SerializeField]
        private RectTransform header;
        [SerializeField]
        private RectTransform chooseBonusText;
        [SerializeField]
        private RectTransform footer;
        [SerializeField]
        private VerticalLayoutGroup containerLayoutGroup;
        
        [SerializeField]
        private float animationSpeed = 1f;
        
        private List<PerkItemView> _realPerkViews;
        private GameStateMachine _gameStateMachine;
        private IAdsService _adsService;
        private BonusesManager _bonusesManager;
        private LettersBagManager _lettersBagManager;
        private TriggersManager _triggersManager;
        private int _reRollsCoinsPrice = 0;
        private int _freeReRollsCount = 0;
        private IAudioService _audioService;
        private ICurrenciesService _currenciesService;

        public event Action OnBonusSelected;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, IAdsService adsService, LettersBagManager lettersBagManager, 
            TriggersManager triggersManager, IAudioService audioService, ICurrenciesService currenciesService)
        {
            _currenciesService = currenciesService;
            _audioService = audioService;
            _triggersManager = triggersManager;
            _lettersBagManager = lettersBagManager;
            _adsService = adsService;
            _gameStateMachine = gameStateMachine;
        }

        public override void Show(object[] inData = null)
        {
            _audioService.PlaySound(SoundKeys.YOU_WIN_STAGE);

            base.Show(inData);
        }

        private void Awake()
        {
            reRollButton.OnClicked += TryReRollForCurrency;
            reRollButtonAds.OnClicked += TryReRollForAds;
        }

        private void OnDestroy()
        {
            reRollButton.OnClicked -= TryReRollForCurrency;
            reRollButtonAds.OnClicked -= TryReRollForAds;
        }

        public void Initialize(BonusesSelectionData payload)
        {
            _bonusesManager = payload.BonusesManager;
            
            for(int i = 0; i < items.Count; i++)
            {
                items[i].Initialize(payload.Bonuses[i], this, _lettersBagManager);
            }    
            
            var activePerks = payload.BonusesManager.GetActivePerks();
            for(int i = 0; i < activePerks.Count; i++)
            {
                fakePerkViews[i].Init(activePerks[i]);
            }

            _realPerkViews = payload.PerkViews;
            additionalTriesText.text = $"+{payload.AdditionalTries.ToString()}";
            rerollPriceText.text = payload.ReRollPrice.ToString();
            _freeReRollsCount = payload.FreeReRollsCount;

            freeReRollsButtonState.SetActive(_freeReRollsCount > 0);
            _reRollsCoinsPrice = payload.ReRollPrice;
            freeReRollsCountText.text = _freeReRollsCount.ToString();
        }

        private void TryReRollForAds()
        {
            if (_adsService.IsRewardedReady())
            {
                _adsService.ShowRewarded();
                _adsService.OnRewardedGranted += GrantReRoll;
                _adsService.OnRewardedSkipped += FailReRoll;
            }
        }

        private void FailReRoll()
        {
            _adsService.OnRewardedGranted -= GrantReRoll;
            _adsService.OnRewardedSkipped -= FailReRoll;
        }

        private void GrantReRoll()
        {
            _adsService.OnRewardedGranted -= GrantReRoll;

            ReRollCurrentBonuses();
        }

        private void TryReRollForCurrency()
        {
            if (_freeReRollsCount > 0)
            {
                _freeReRollsCount--;
                freeReRollsButtonState.SetActive(_freeReRollsCount > 0);
                freeReRollsCountText.text = _freeReRollsCount.ToString();
                ReRollCurrentBonuses();
                return;
            }
            if (_currenciesService.GetCurrencyWallet(CurrencyType.COINS).GetAmount() >= _reRollsCoinsPrice)
            {
                _currenciesService.GetCurrencyWallet(CurrencyType.COINS).AddAmount(-_reRollsCoinsPrice);
                ReRollCurrentBonuses();
            }
        }

        private void ReRollCurrentBonuses()
        {
            _audioService.PlaySound(SoundKeys.PERKS_REFRESH);

            var newBonuses = _bonusesManager.SelectBonusesToChooseFrom(3);
            
            for(int i = 0; i < items.Count; i++)
            {
                items[i].Initialize(newBonuses[i], this, _lettersBagManager);
            }
        }

        private void FinishPerkSelection()
        {
            _triggersManager.ProcessTriggersWave(TriggerWave.AFTER_BONUSES_SELECTION, this, .5f);
            _gameStateMachine.Enter<GameplayState>();
        }

        public async void SelectBonus(BonusItemToSelectView selectableGameplayBonus)
        {
            OnBonusSelected?.Invoke();
            
            switch (selectableGameplayBonus.SelectableGameplayBonus.GameplayBonusType)
            {
                case GameplayBonusType.PERK:
                    AnimatePerkSelection(selectableGameplayBonus);
                    break;
                case GameplayBonusType.TILE:
                    AnimateTileSelection(selectableGameplayBonus);
                    break;
                case GameplayBonusType.RULE:
                    AnimatePerkSelection(selectableGameplayBonus);
                    break;
            }
        }

        private async UniTask AnimatePerkSelection(BonusItemToSelectView selectableGameplayBonus)
        {
            horizontalLayoutGroup.enabled = false;
            foreach (var perkView in _realPerkViews)
            {
                if (perkView.Perk == null)
                {
                    foreach (var item in items)
                    {
                        if (item != selectableGameplayBonus)
                        {
                            item.GetComponent<CustomButton>().ChangeInteractable(false);
                        }
                    }
                    AnimateBonusFlight(selectableGameplayBonus, perkView);
                    break;
                }
            }
            
            //await UniTask.WaitForSeconds(.3f);
            
        }

        private async UniTask AnimateBonusFlight(BonusItemToSelectView selectableGameplayBonus, PerkItemView perkView)
        {
            gameObject.SetActive(false);
            selectableGameplayBonus.transform.SetParent(transform.parent, true);
            FinishPerkSelection();
            await selectableGameplayBonus.AnimateBonusSelectionWithArc(perkView.GetComponent<RectTransform>());
            _audioService.PlaySound(SoundKeys.PERK_TAKEN);
            _bonusesManager.AddBonus(selectableGameplayBonus.SelectableGameplayBonus);
            Destroy(selectableGameplayBonus);
            Close();
        }
        
        private async UniTask AnimateTileSelection(BonusItemToSelectView selectableGameplayBonus)
        {
            foreach (var item in items)
            {
                if (item != selectableGameplayBonus)
                {
                    item.GetComponent<CustomButton>().ChangeInteractable(false);
                }
            }
            
            
            await AnimateAndApplyTileBonus(selectableGameplayBonus);
            
            _audioService.PlaySound(SoundKeys.BAG_LETTERS_ADDED);

            _lettersBagManager.UpdateLettersBagInfo();
            
            Close();
        }

        private async UniTask AnimateAndApplyTileBonus(BonusItemToSelectView selectableGameplayBonus)
        {
            _bonusesManager.AddBonus(selectableGameplayBonus.SelectableGameplayBonus);
            
            UniTask task = default;
            task = selectableGameplayBonus.AnimateBonusSelectionWithLine(_lettersBagManager.GetLettersBagView().GetComponent<RectTransform>());
            
            containerLayoutGroup.enabled = false;

            var selectedIndex = items.IndexOf(selectableGameplayBonus);
            
            float nudgeDuration = 0.25f / animationSpeed;
            float mainMovementDuration = 0.6f / animationSpeed;

            var sequence = DOTween.Sequence();

            /*sequence.Append(header.DOAnchorPosX(header.anchoredPosition.x - 25, nudgeDuration).SetEase(Ease.InOutQuad));
            sequence.Join(perksContainer.DOAnchorPosX(perksContainer.anchoredPosition.x + 25, nudgeDuration)
                .SetEase(Ease.InOutQuad));
            sequence.Join(chooseBonusText.DOAnchorPosX(chooseBonusText.anchoredPosition.x + 25, nudgeDuration)
                .SetEase(Ease.InOutQuad));
            sequence.Join(footer.DOAnchorPosX(footer.anchoredPosition.x - 25, nudgeDuration).SetEase(Ease.InOutQuad));

            for (var i = 0; i < items.Count; i++)
            {
                if (i == selectedIndex) continue;

                var itemRect = items[i].GetComponent<RectTransform>();
                var direction = i < selectedIndex ? 1 : -1;

                sequence.Join(itemRect
                    .DOAnchorPosX(itemRect.anchoredPosition.x + direction * 25f, nudgeDuration)
                    .SetEase(Ease.InOutQuad));
            }*/

            //sequence.InsertCallback(nudgeDuration * 2, () =>
            //{
                //task = selectableGameplayBonus.AnimateBonusSelectionWithLine(_lettersBagManager.GetLettersBagView().GetComponent<RectTransform>());
            //});
            
            sequence.Append(header.DOAnchorPosX(header.anchoredPosition.x + header.sizeDelta.x, mainMovementDuration)
                .SetEase(Ease.OutSine));
            sequence.Join(perksContainer
                .DOAnchorPosX(perksContainer.anchoredPosition.x - perksContainer.sizeDelta.x, mainMovementDuration)
                .SetEase(Ease.OutSine));
            sequence.Join(chooseBonusText
                .DOAnchorPosX(chooseBonusText.anchoredPosition.x - chooseBonusText.sizeDelta.x, mainMovementDuration)
                .SetEase(Ease.OutSine));
            sequence.Join(footer.DOAnchorPosX(footer.anchoredPosition.x + footer.sizeDelta.x, mainMovementDuration)
                .SetEase(Ease.OutSine));

            for (var i = 0; i < items.Count; i++)
            {
                if (i == selectedIndex) continue;

                var itemRect = items[i].GetComponent<RectTransform>();
                var direction = i < selectedIndex ? -1 : 1;

                sequence.Join(itemRect
                    .DOAnchorPosX(itemRect.anchoredPosition.x + itemRect.sizeDelta.x * direction * 3, mainMovementDuration)
                    .SetEase(Ease.OutSine));
            }

            await sequence.AsyncWaitForCompletion();
            selectableGameplayBonus.transform.SetParent(transform.parent, true);
            FinishPerkSelection();
            gameObject.SetActive(false);
            
            await UniTask.WhenAll(task);
            Destroy(selectableGameplayBonus);
        }

    }
}