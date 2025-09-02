using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Pooling
{
    public interface IObjectPool
    {
        public async UniTask<T> GetObjectAsync<T>(T prefab) where T : Component, IPoolable
        {
            return null;
        }

        public async UniTask WarmupPoolAsync<T>(T prefab, int count) where T : Component, IPoolable
        {
            
        }

        public int GetReadyObjectsCount<T>(T prefab) where T : Component, IPoolable;
    }
}