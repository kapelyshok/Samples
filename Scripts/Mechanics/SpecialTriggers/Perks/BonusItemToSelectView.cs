using System;
using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using AtomicApps.UI.Mechanics;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AtomicApps.UI.Popups
{
    public class BonusItemToSelectView : MonoBehaviour
    {
        [SerializeField]
        private List<GameplayBonusTypeItemData> bonusItemDatas = new List<GameplayBonusTypeItemData>();
        [SerializeField]
        private List<RarityItemData> rarityItemDatas = new List<RarityItemData>();
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private TileEntryView tileEntry;
        [SerializeField]
        private CustomButton customButton;
        [SerializeField]
        private Image image;

        [SerializeField]
        private float animationSpeed = 1f;

        private StageCompletedPopup _stageCompletedPopup;
        private RectTransform _rectTransform;
        private LettersBagManager _lettersBagManager;

        public ISelectableGameplayBonus SelectableGameplayBonus { get; private set; }
        
        public void Initialize(ISelectableGameplayBonus payloadBonus, StageCompletedPopup stageCompletedPopup,
            LettersBagManager lettersBagManager)
        {
            _lettersBagManager = lettersBagManager;
            customButton.OnClicked -= SelectBonus;

            _stageCompletedPopup = stageCompletedPopup;
            SelectableGameplayBonus = payloadBonus;
            _rectTransform = GetComponent<RectTransform>();
            
            foreach (GameplayBonusTypeItemData itemData in bonusItemDatas)
            {
                itemData.Background.SetActive(itemData.GameplayBonusType == payloadBonus.GameplayBonusType);
            }
            foreach (RarityItemData rarityItemData in rarityItemDatas)
            {
                rarityItemData.Text.SetActive(rarityItemData.Rarity == payloadBonus.Rarity);
            }

            descriptionText.text = payloadBonus.TextDetailed;
            image.sprite = payloadBonus.Icon;

            customButton.OnClicked += SelectBonus;

            TryEnableTileInfo(payloadBonus);
        }

        private void TryEnableTileInfo(ISelectableGameplayBonus payloadBonus)
        {
            if (payloadBonus is not BaseSpecialTileSO tileSo)
            {
                tileEntry.gameObject.SetActive(false);
                return;
            }
            var nextRandomLetterEntry = _lettersBagManager.PeekNextRandomLetterEntry();
            var newTileEntry = new TileEntry() {LetterEntry = nextRandomLetterEntry, Tile = tileSo.Tile};
            
            tileEntry.gameObject.SetActive(true);
            tileEntry.EnableSpecialTile(newTileEntry);
            tileEntry.ChangeLetter(nextRandomLetterEntry.Letter);
            tileEntry.ChangeScore(nextRandomLetterEntry.Points.ToString());
        }

        public async UniTask AnimateBonusSelectionWithArc(RectTransform target)
        {
            var animator = GetComponent<ButtonAnimator>();
            await UniTask.WaitForSeconds(0.15f);
            animator.enabled = false;

            float pullBackDistance = 50f;
            float pullBackDuration = 0.55f / animationSpeed;
            float launchDuration = 0.65f / animationSpeed;
            float arcHeight = 900f;
            float arcSideOffset = Random.Range(-900f, 900f);

            Sequence sequence = DOTween.Sequence();
            
            float maxRotation = 15f;
            float targetZRotation = Mathf.Sign(arcSideOffset) * maxRotation;
            
            var slingTween = _rectTransform
                .DOAnchorPosY(_rectTransform.anchoredPosition.y - pullBackDistance, pullBackDuration)
                .SetEase(Ease.OutCubic);
            var rotateTween = _rectTransform
                .DORotate(new Vector3(0, 0, targetZRotation), pullBackDuration)
                .SetEase(Ease.OutCubic);
            var scaleUpTween = _rectTransform
                .DOScale(new Vector3(1.1f, 1.1f, 1.1f), pullBackDuration)
                .SetEase(Ease.OutCubic);
            
            sequence.Append(slingTween);
            sequence.Join(rotateTween);
            sequence.Join(scaleUpTween);

            Vector3 start = _rectTransform.position;
            Vector3 end = target.position;

            Vector3 control = (start + end) / 2f + Vector3.up * arcHeight + Vector3.right * arcSideOffset;

            float t = 0f;

            _rectTransform.localScale = Vector3.one;

            Tween moveTween = DOTween.To(() => t, x => t = x, 1f, launchDuration)
                .SetEase(Ease.InOutQuad)
                .OnUpdate(() =>
                {
                    float oneMinusT = 1f - t;

                    Vector3 pos = oneMinusT * oneMinusT * start
                                  + 2f * oneMinusT * t * control
                                  + t * t * end;

                    _rectTransform.position = pos;
                });

            Tween scaleTween = _rectTransform
                .DOScale(Vector3.zero, launchDuration)
                .SetEase(Ease.InOutQuad);

            sequence.Append(moveTween);
            sequence.Join(scaleTween);

            await sequence.AsyncWaitForCompletion();
        }
        
        public async UniTask AnimateBonusSelectionWithLine(RectTransform target)
        {
            var animator = GetComponent<ButtonAnimator>();
            await UniTask.WaitForSeconds(0.15f);
            animator.enabled = false;

            float pullBackDistance = 90f;
            float pullBackDuration = 0.55f / animationSpeed;
            float launchDuration = 0.65f / animationSpeed;

            Sequence sequence = DOTween.Sequence();
            
            float maxRotation = 15f;
            float targetZRotation = maxRotation;
            Vector3 currentPos = _rectTransform.position;
            Vector3 targetPos = target.position;

            Vector3 directionToTarget = (targetPos - currentPos).normalized;
            Vector3 slingOffset = -directionToTarget * pullBackDistance;
            Vector3 slingTargetPos = currentPos + slingOffset;
            
            var slingTween = _rectTransform
                .DOMove(slingTargetPos, pullBackDuration)
                .SetEase(Ease.OutCubic);
            
            var rotateTween = _rectTransform
                .DORotate(new Vector3(0, 0, targetZRotation), pullBackDuration)
                .SetEase(Ease.OutCubic);
            var scaleUpTween = _rectTransform
                .DOScale(new Vector3(1.1f, 1.1f, 1.1f), pullBackDuration)
                .SetEase(Ease.OutCubic);
            
            sequence.Append(slingTween);
            sequence.Join(rotateTween);
            sequence.Join(scaleUpTween);
            
            Tween scaleTween = _rectTransform
                .DOScale(Vector3.zero, launchDuration)
                .SetEase(Ease.InOutQuad);
            
            Tween moveTween = _rectTransform
                .DOMove(target.position, launchDuration)
                .SetEase(Ease.InOutQuad);

            sequence.Append(moveTween);
            sequence.Join(scaleTween);

            await sequence.AsyncWaitForCompletion();
        }
        
        private void OnDestroy()
        {
            customButton.OnClicked -= SelectBonus;
        }

        private void SelectBonus()
        {
            customButton.OnClicked -= SelectBonus;
            _stageCompletedPopup.SelectBonus(this);
        }
    }

    [Serializable]
    internal class GameplayBonusTypeItemData
    {
        public GameplayBonusType GameplayBonusType;
        public GameObject Background;
    }
    
    [Serializable]
    internal class RarityItemData
    {
        public Rarity Rarity;
        public GameObject Text;
    }
}