using System;
using DG.Tweening;
using UnityEngine;

namespace AtomicApps.Infrastructure
{
    public class LoadingCurtain : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;

            DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 1f, .5f)
                .SetEase(Ease.Linear);
        }

        public void Hide()
        {
            _canvasGroup.alpha = 1f;

            DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0f, .5f)
                .SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
        }
    }
}