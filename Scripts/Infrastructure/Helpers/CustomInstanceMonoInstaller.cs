using UnityEngine;
using Zenject;

namespace AtomicApps.Scripts.Infrastructure.Helpers
{
    public class CustomInstanceMonoInstaller<TReference,TInterface> : MonoInstaller where TReference : TInterface
    {
        [SerializeField]
        private TReference installedObject;

        public override void InstallBindings()
        {
            Container.Bind<TInterface>().FromInstance(installedObject).AsSingle().NonLazy();
        }
    }
}