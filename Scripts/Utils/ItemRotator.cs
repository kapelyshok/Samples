using DG.Tweening;
using UnityEngine;

namespace AtomicApps.Utils
{
    public class ItemRotator : MonoBehaviour
    {
        [SerializeField] private bool rotateOnEnable = true;

        [Space]
        [SerializeField] private Vector3 rotateVector = new Vector3(0,0,-360);
        [SerializeField, Min(0.1f)] private float duration = 15f;
        [SerializeField] private int loops = -1;

        private Tween _rotateTween;

        private void OnEnable()
        {
            if (rotateOnEnable) Rotate();
        }

        public void Rotate()
        {
            transform.rotation = Quaternion.identity;

            _rotateTween?.Kill();
            _rotateTween = transform.DORotate(rotateVector, duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(loops, LoopType.Restart)
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private void OnDisable()
        {
            _rotateTween?.Kill();
        }
    }
}
