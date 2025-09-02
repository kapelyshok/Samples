using System;
using UnityEngine;

namespace AtomicApps.Pooling
{
    public interface IPoolable : IResettable
    {
        public GameObject GameObject { get; }

        public event Action<IPoolable> Destroyed;

        public void Release();
    }
}
