using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicApps.Utils
{
    public class StarsAnimator : MonoBehaviour
    {
        [SerializeField]
        private List<Image> stars = new List<Image>();

        [SerializeField]
        private bool startOnEnable = true;

        [Header("Animation Settings")]
        [SerializeField, Min(0.1f)]
        private float scaleDuration = 0.3f;

        [SerializeField]
        private float scaleUpFactor = 1.2f;

        [SerializeField]
        private float scaleDownFactor = 0.8f;

        [SerializeField]
        private float alphaMin = 0.5f;

        [SerializeField]
        private float alphaBlinkDuration = 0.2f;

        [SerializeField]
        private Vector2 delayBetweenAnimations = new Vector2(0.2f, 1.0f);

        private bool _isAnimating;
        private List<Tween> _runningTweens = new List<Tween>();

        private void OnEnable()
        {
            if (startOnEnable)
            {
                StartAnimation();
            }
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        public void StartAnimation()
        {
            if (_isAnimating) return;
            _isAnimating = true;
            StartCoroutine(AnimateStarsLoop());
        }

        public void StopAnimation()
        {
            _isAnimating = false;
            StopAllCoroutines();
            foreach (var tween in _runningTweens)
            {
                tween.Kill();
            }
            _runningTweens.Clear();
        }

        private IEnumerator AnimateStarsLoop()
        {
            while (_isAnimating)
            {
                if (stars.Count == 0) yield break;

                // Pick a random star
                Image star = stars[Random.Range(0, stars.Count)];

                // Decide randomly if scaling up or down
                bool scaleUp = Random.value > 0.5f;
                float targetScale = scaleUp ? scaleUpFactor : scaleDownFactor;

                // Scale animation
                Tween scaleTween = star.rectTransform
                    .DOScale(targetScale, scaleDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        star.rectTransform.DOScale(1f, scaleDuration).SetEase(Ease.InQuad);
                    });
                _runningTweens.Add(scaleTween);

                // Random chance to blink alpha
                if (Random.value > 0.5f)
                {
                    Tween alphaTween = star
                        .DOFade(alphaMin, alphaBlinkDuration)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetEase(Ease.InOutQuad);
                    _runningTweens.Add(alphaTween);
                }

                // Wait a random delay before animating the next star
                float delay = Random.Range(delayBetweenAnimations.x, delayBetweenAnimations.y);
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
