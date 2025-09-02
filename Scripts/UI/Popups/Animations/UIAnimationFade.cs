using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AtomicApps.UI.Popups
{
    [CreateAssetMenu(fileName = "Fade", menuName = "UIAnimations/UIAnimationFade")]
    public class UIAnimationFade : UIAnimation
    {
        public override async UniTask ActivateAsync(GameObject target, bool inAnimation, Action onCompletedCallback = null)
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
            
            if (inAnimation)
            {
                canvasGroup.alpha = 0f;
                sequence.Append(canvasGroup.DOFade(1f, duration)).SetEase(ease);
            }
            else
            {
                canvasGroup.alpha = 1f;
                sequence.Append(canvasGroup.DOFade(0f, duration)).SetEase(ease);
            }

            sequence.OnComplete(() =>
            {
                FireAnimationFinished();
                onCompletedCallback?.Invoke();
            });
        }
    }
}