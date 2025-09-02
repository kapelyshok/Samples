using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicApps.Utils
{
    public class RawImageScroller : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Vector2 uvSpeed = new Vector2(-0.012f, -0.02f); // TR -> BL
        private bool _isAnimated = false;

        private void Reset()
        {
            rawImage = GetComponent<RawImage>();
        }

        public async UniTask AnimateRapidReverse(float seconds, bool returnPreviousDirection = false)
        {
            uvSpeed *= -25;
            await UniTask.WaitForSeconds(seconds);
            if (returnPreviousDirection)
            {
                uvSpeed /= -25;
            }
            else
            {
                uvSpeed /= 25;
            }
        }

        private void Update()
        {
            if (!rawImage) return;
            var r = rawImage.uvRect;
            r.x += uvSpeed.x * Time.unscaledDeltaTime;
            r.y += uvSpeed.y * Time.unscaledDeltaTime;
            rawImage.uvRect = r;
        }
    }
}
