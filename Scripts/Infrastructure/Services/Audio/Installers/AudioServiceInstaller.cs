using AtomicApps.Infrastructure.Services.Audio;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.Services.Audio.Installers
{
    public class AudioServiceInstaller : MonoInstaller
    {
        [SerializeField]
        private AudioService audioService;

        public override void InstallBindings()
        {
            Container.Bind<IAudioService>().FromInstance(audioService).AsSingle().NonLazy();
        }
    }
}