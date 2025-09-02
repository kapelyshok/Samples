using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.UI
{
    public class ClaimItemsWithCircleAnimation : BaseClaimItemsWithAnimation<AnimatedCollectableItem>
    {
        [SerializeField, Min(0f)] protected float spreadCircleRadius = 30;
        [SerializeField, Min(0f)] private float delayBeforeMovement = 0f;

        private Sprite _iconSprite;
        private Vector2 _iconSize;
        
        public async UniTask StartAnimation(float delayBeforeMovement = 0f)
        {
            this.delayBeforeMovement = delayBeforeMovement;
            SpawnItems();
        }

        protected override void SetupItemForAnimation(AnimatedCollectableItem item)
        {
            var destinationPosition = _destinationPosition;
            var spreadRadius = item.transform.position + (Vector3)(Random.insideUnitCircle * spreadCircleRadius);

            if (_iconSprite != null)
            {
                item.SetSprite(_iconSprite);
                item.Icon.rectTransform.sizeDelta = _iconSize;
            }
            
            item.AddDestination(spreadRadius, destinationPosition);
            WaitAndMove(item);
        }

        private async UniTask WaitAndMove(AnimatedCollectableItem item)
        {
            await UniTask.WaitForSeconds(delayBeforeMovement);
            item.ShouldAnimate(true);
        }

        public void SetSprite(Sprite sprite) => _iconSprite = sprite;
        public void SetIconSize(Vector2 size) => _iconSize = size;

        public void SetDestinationPoint(RectTransform destinationPoint)
        {
            this.destinationPoint = destinationPoint;
            _destinationPosition = destinationPoint.position;
        }

        public void SetSpawnPoint(RectTransform spawnPoint)
        {
            this.spawnPoint = spawnPoint;
            _spawnPosition = spawnPoint.position;
        }
        
        public void SetAmount(int amount) => spawnAmount = amount;
    }
}
