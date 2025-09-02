using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public class AdsServiceInstaller : MonoInstaller
    {
        [SerializeField] private AdsService adsService;

        public override void InstallBindings()
        {
            Container.Bind<IAdsService>().FromInstance(adsService).AsSingle();
        }
    }
}
