using AtomicApps.Infrastructure.Services.AnalyticsService;
using AtomicApps.Sdk;
using UnityEngine;
using Zenject;

namespace SDK.Installers
{
    public class AnalyticsServiceInstaller : MonoInstaller
    {
        [SerializeField] private AnalyticsService analyticsService;

        public override void InstallBindings()
        {
            //Container.Bind<IAnalyticsService>().FromInstance(analyticsService).AsSingle();
        }
    }
}