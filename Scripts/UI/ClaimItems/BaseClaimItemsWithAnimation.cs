using System;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Pooling;
using Cysharp.Threading.Tasks;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps.UI
{
    public abstract class BaseClaimItemsWithAnimation<T> : MonoBehaviour where T : BaseAnimatedCollectableItem
    {
        [Header("Components")]
        [SerializeField] protected Button claimButton;

        [Space]
        [SerializeField] protected T prefab;
        [SerializeField] protected RectTransform spawnPoint;
        [SerializeField] protected RectTransform destinationPoint;

        [Header("Parameters")]
        [SerializeField, Min(0)] protected int spawnAmount = 10;

        [Inject] protected IObjectPool _objectPool;

        protected Vector3 _spawnPosition = Vector3.zero;
        protected Vector3 _destinationPosition = Vector3.zero;
        
        protected Dictionary<int, List<T>> _itemsInAction = new ();
        protected Dictionary<int, int> _remainingInBatch = new();
        protected int _currentAnimationsCount = -1;
        private bool _isFirstItemFinishSent = false;
        
        public event Action<List<T>> OnAnimationComplete; 
        public event Action OnFirstItemAnimationComplete; 

        protected virtual void Start()
        {
            _spawnPosition = spawnPoint.position;
            _destinationPosition = destinationPoint.position;
            
            if (claimButton != null)
                claimButton.onClick.AddListener(SpawnItems);
        }

        protected virtual async void SpawnItems()
        {
            _isFirstItemFinishSent = false;
            _currentAnimationsCount = GetNextAvailableBatchId();
            
            int batchId = _currentAnimationsCount;
            var items = new List<T>();
            _remainingInBatch[batchId] = spawnAmount;
            
            for (int i = 0; i < spawnAmount; ++i)
            {
                var spawnPosition = _spawnPosition;
                var item = await GetNewItem(spawnPosition);
                
                item.OnDestinationArrived += ItemDestinationArrivedHandler;

                items.Add(item);
                SetupItemForAnimation(item);
            }
            
            _itemsInAction.Add(_currentAnimationsCount, items);
        }
        
        private int GetNextAvailableBatchId()
        {
            int id = 0;
            while (_itemsInAction.ContainsKey(id))
            {
                id++;
            }
            return id;
        }

        private void ItemDestinationArrivedHandler(BaseAnimatedCollectableItem item)
        {
            if (!_isFirstItemFinishSent)
            {
                _isFirstItemFinishSent = true;
                OnFirstItemAnimationComplete?.Invoke();
            }
            
            item.OnDestinationArrived -= ItemDestinationArrivedHandler;
            
            foreach (var kvp in _itemsInAction)
            {
                int batchId = kvp.Key;
                List<T> items = kvp.Value;

                if (items.Contains(item))
                {
                    _remainingInBatch[batchId]--;

                    if (_remainingInBatch[batchId] <= 0)
                    {
                        OnAnimationComplete?.Invoke(items);
                        _itemsInAction.Remove(batchId);
                        _remainingInBatch.Remove(batchId);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// All final custom preparations for items
        /// </summary>
        /// <param name="item"></param>
        protected abstract void SetupItemForAnimation(T item);

        private async UniTask<T> GetNewItem(Vector3 spawnPosition)
        {
            T item = await _objectPool.GetObjectAsync<T>(prefab);

            item.transform.SetParent(transform);
            item.transform.position = spawnPosition;
            item.transform.rotation = Quaternion.identity;

            return item;
        }

        public void SetSpawnPosition(Vector3 position) => _spawnPosition = position;

        public void SetDestinationPosition(Vector3 position) => _destinationPosition = position;

        private void OnDestroy()
        {
            if (claimButton != null)
                claimButton.onClick.RemoveListener(SpawnItems);
        }
    }
}