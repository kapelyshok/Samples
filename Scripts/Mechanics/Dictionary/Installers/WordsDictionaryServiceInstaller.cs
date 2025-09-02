using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.Dictionary
{
    public class WordsDictionaryServiceInstaller : MonoInstaller
    {
        [SerializeField]
        private WordsDictionaryService wordsDictionaryService;

        public override void InstallBindings()
        {
            Container.Bind<IWordsDictionaryService>().FromInstance(wordsDictionaryService).AsSingle().NonLazy();
        }
    }
}
