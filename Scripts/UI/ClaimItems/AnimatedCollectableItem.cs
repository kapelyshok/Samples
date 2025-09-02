using UnityEngine;
using UnityEngine.UI;

namespace AtomicApps.UI
{
    public class AnimatedCollectableItem : BaseAnimatedCollectableItem
    {
        [SerializeField] private Image icon;
        [SerializeField, Min(0f)] private float speed = 5f;

        public Image Icon => icon;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
        }
        
        private void Update()
        {
            MoveItem();
        }
        
        protected override void MoveItem()
        {
            if(!_shouldAnimate) return;
            
            if (_currentDestinationIndex >= _destinations.Count)
            {
                NotifyOnDestinationArrived();
                Release();
                return;
            }

            _rectTransform.position = Vector3.Slerp(_rectTransform.position, _destinations[_currentDestinationIndex],
                speed * Time.deltaTime);

            if (_currentDestinationIndex == _destinations.Count - 1)
            {
                _rectTransform.localScale =
                    Vector3.Lerp(_rectTransform.localScale, Vector3.zero, speed / 2 * Time.deltaTime);
            }

            if (Vector3.Distance(_rectTransform.position, _destinations[_currentDestinationIndex]) <= 10f)
                _currentDestinationIndex++;
        }

        public void SetSprite(Sprite sprite)
        {
            icon.sprite = sprite;
        } 

        private void OnDisable()
        {
            _currentDestinationIndex = 0;

            ShouldAnimate(false);
            _destinations.Clear();
        }
    }
}