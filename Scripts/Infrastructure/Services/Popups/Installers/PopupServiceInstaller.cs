using AtomicApps.Infrastructure.Services.Popups;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastracture.Services.Popups.Installers
{
    public class PopupServiceInstaller : MonoInstaller
    {
        [SerializeField]
        private PopupService popupService;

        public override void InstallBindings()
        {
            Container.Bind<IPopupService>().FromInstance(popupService).AsSingle().NonLazy();
        }
    }
}
