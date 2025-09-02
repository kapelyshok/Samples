using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AtomicApps.UI.Popups
{
    [CreateAssetMenu(fileName = "Zoom", menuName = "UIAnimations/" + nameof(UIAnimationZoom))]
    public class UIAnimationZoom : UIAnimation
    {
        public override async UniTask ActivateAsync(GameObject target, bool inAnimation,
            Action onCompletedCallback = null)
        {
            if (target == null)
            {
                Debug.LogError("Can't perform UI animation. Target is null");
                return;
            }

            if (!target.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = target.AddComponent<CanvasGroup>();
            }
            
            var sequence = DOTween.Sequence();
            var expandedSize = new Vector3(1.12f, 1.12f, 1.12f);
            
            if (inAnimation)
            {
                sequence.Append(target.transform.DOScale(expandedSize, duration * 0.8f).
                    From(Vector3.zero)).SetEase(ease);
                sequence.Join(canvasGroup.DOFade(1, duration * 0.8f).From(0f)).SetEase(ease);
                sequence.Append(target.transform.DOScale(Vector3.one, duration * 0.2f)).SetEase(ease);
            }
            else
            {
                sequence.Append(target.transform.DOScale(expandedSize, duration * 0.2f).
                    From(Vector3.one)).SetEase(ease);
                sequence.Append(target.transform.DOScale(Vector3.zero, duration * 0.8f)).SetEase(ease);
                sequence.Join(canvasGroup.DOFade(0, duration * 0.8f).From(1f)).SetEase(ease);
            }

            sequence.OnComplete(() =>
            {
                FireAnimationFinished();
                onCompletedCallback?.Invoke();
            });
        }
    }
}