using System;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.Pooling;
using AtomicApps.UI.Mechanics;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class PerkItemView : PoolableMonoBehaviour
    {
        [SerializeField]
        private Image iconSmall;
        [SerializeField]
        private Image iconBig;
        [SerializeField]
        private TextMeshProUGUI smallText;
        [SerializeField]
        private GameObject lockedState;
        [SerializeField]
        private float animationSpeed = 1f;
        [SerializeField]
        private RectTransform tileShowPlace;
        [SerializeField]
        private CustomButton customButton;
        
        [Header("Description Bubble")]
        [SerializeField]
        private CanvasGroup descriptionBubble;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private Button descriptionButton;
        
        private PerkSO _perk;
        private RectTransform _rectTransform;
        private IPopupService _popupService;
        private IAudioService _audioService;

        public PerkSO Perk => _perk;

        public bool IsLocked { get; private set; } = true;

        [Inject]
        private void Construct(IPopupService popupService, IAudioService audioService)
        {
            _audioService = audioService;
            _popupService = popupService;
        }

        private void Awake()
        {
            customButton.OnClicked += ShowInfoPopup;
            descriptionButton.onClick.AddListener(CloseHintBubble);
        }

        private void OnDestroy()
        {
            customButton.OnClicked -= ShowInfoPopup;
            descriptionButton.onClick.RemoveListener(CloseHintBubble);
        }

        private void ShowInfoPopup()
        {
            _audioService.PlaySound(SoundKeys.TAP_OPEN);

            if(_perk == null) return;

            ShowHintBubbleAsync();
            //_popupService.ShowPopupAsync(PopupKeys.PERK_INFO_POPUP, UIConstants.PopupShow.ShowOver, _perk.TextDetailed);
        }

        public void Init(PerkSO perk)
        {
            _perk = perk;

            if (string.IsNullOrEmpty(perk.TextSmall))
            {
                iconBig.gameObject.SetActive(true);
                iconSmall.gameObject.SetActive(false);
                iconBig.sprite = _perk.Icon;
            }
            else
            {
                iconBig.gameObject.SetActive(false);
                iconSmall.gameObject.SetActive(true);
                iconSmall.sprite = _perk.Icon;
                smallText.text = _perk.TextSmall;
            }
            
            _rectTransform = GetComponent<RectTransform>();

            IsLocked = false;
            lockedState.SetActive(false);
            descriptionText.text = perk.TextDetailed;
        }

        public async UniTask ShowHintBubbleAsync()
        {
            await UniTask.WaitForSeconds(.1f);
            descriptionBubble.alpha = 0f;
            descriptionBubble.gameObject.SetActive(true);
            await descriptionBubble.DOFade(1f, .5f / animationSpeed).AsyncWaitForCompletion();
        }

        private void CloseHintBubble()
        {
            CloseHintBubbleAsync();
        }
        
        public async UniTask CloseHintBubbleAsync()
        {
            await descriptionBubble.DOFade(0f, .5f / animationSpeed).AsyncWaitForCompletion();
            descriptionBubble.gameObject.SetActive(false);
        }

        public RectTransform GetTileShowPlace()
        {
            return tileShowPlace;
        }
        
        public async UniTask InitWithAnimation(PerkSO perk)
        {
            _perk = perk;
            
            if (string.IsNullOrEmpty(perk.TextSmall))
            {
                iconBig.gameObject.SetActive(true);
                iconSmall.gameObject.SetActive(false);
                iconBig.sprite = _perk.Icon;
            }
            else
            {
                iconBig.gameObject.SetActive(false);
                iconSmall.gameObject.SetActive(true);
                iconSmall.sprite = _perk.Icon;
                smallText.text = _perk.TextSmall;
            }
            
            _rectTransform = GetComponent<RectTransform>();

            IsLocked = false;
            lockedState.SetActive(false);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .3f / animationSpeed)).SetEase(Ease.InOutSine);
            sequence.Append(_rectTransform.DOScale(Vector3.one, .2f / animationSpeed));
            await sequence.AsyncWaitForCompletion();
        }
        
        public async UniTask AnimateSuccess()
        {
            Vector3 originalScale = _rectTransform.localScale;

            float punchScale = 1.3f;
            float duration = 0.5f;

            _audioService.PlaySound(SoundKeys.PERK_ACTIVATED);

            await _rectTransform
                .DOScale(originalScale * punchScale, duration / 2 / animationSpeed)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _rectTransform.DOScale(originalScale, duration / 2 / animationSpeed).SetEase(Ease.InQuad);
                })
                .AsyncWaitForCompletion();
        }

        public async UniTask AnimateFailure()
        {
            
        }
    }
}