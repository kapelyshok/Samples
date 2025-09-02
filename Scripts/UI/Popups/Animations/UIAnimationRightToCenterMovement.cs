using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AtomicApps.UI.Popups
{
    [CreateAssetMenu(fileName = "RightToCenterMovement", menuName = "UIAnimations/UIAnimationRightToCenterMovement")]
    public class UIAnimationRightToCenterMovement : UIAnimation
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
            var startPosition = new Vector3(Screen.width, 0, 0);

            if (inAnimation)
            {
                sequence.Append(target.transform.DOLocalMove(Vector3.zero, duration).
                    From(startPosition).SetEase(ease));
                sequence.Join(canvasGroup.DOFade(1f, duration).From(0f).SetEase(ease));
            }
            else
            {
                sequence.Append(canvasGroup.DOFade(0f, duration).From(1f).SetEase(ease));
                sequence.Join(target.transform.DOLocalMove(startPosition, duration).
                    SetEase(ease));
            }

            sequence.OnComplete(() =>
            {
                FireAnimationFinished();
                onCompletedCallback?.Invoke();
            });        
        }
    }
}