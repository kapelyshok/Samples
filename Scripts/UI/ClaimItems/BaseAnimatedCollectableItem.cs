using System;
using System.Collections.Generic;
using AtomicApps.Pooling;
using UnityEngine;

namespace AtomicApps.UI
{
    public abstract class BaseAnimatedCollectableItem : PoolableMonoBehaviour
    {
        protected readonly List<Vector2> _destinations = new();
        protected int _currentDestinationIndex;
        protected bool _shouldAnimate;
        
        public event Action<BaseAnimatedCollectableItem> OnDestinationArrived;

        /// <summary>
        /// Implement movement logic and don't forget to call NotifyOnDestinationArrived() at the end.
        /// </summary>
        protected abstract void MoveItem();
        
        public void AddDestination(params Vector2[] destinations)
        {
            if (destinations == null || destinations.Length == 0) return;

            _destinations.AddRange(destinations);
        }
        
        public void ShouldAnimate(bool should) => _shouldAnimate = should;

        protected void NotifyOnDestinationArrived()
        {
            OnDestinationArrived?.Invoke(this);
        }
    }
}