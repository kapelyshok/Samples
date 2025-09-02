using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure
{
    public class SceneLoaderServiceInstaller : MonoInstaller
    {
        [SerializeField]
        private SceneLoaderService sceneLoaderService;

        public override void InstallBindings()
        {
            Container.Bind<SceneLoaderService>().FromInstance(sceneLoaderService).AsSingle().NonLazy();
        }
    }
}