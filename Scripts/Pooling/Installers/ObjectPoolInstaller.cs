using UnityEngine;
using Zenject;

namespace AtomicApps.Pooling
{
    public class ObjectPoolInstaller : MonoInstaller
    {
        [SerializeField]
        private ObjectPool objectPool;

        public override void InstallBindings()
        {
            Container.Bind<IObjectPool>().FromInstance(objectPool).AsSingle().NonLazy();
        }
    }
}