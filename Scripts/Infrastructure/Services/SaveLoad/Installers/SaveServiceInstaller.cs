using AtomicApps.Infrastructure.Services.SaveLoad;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.Services.SaveLoad.Installers
{
    public class SaveServiceInstaller : MonoInstaller
    {
        [SerializeField]
        private SaveService saveService;

        public override void InstallBindings()
        {
            Container.Bind<ISaveService>().FromInstance(saveService).AsSingle().NonLazy();
        }
    }
}
