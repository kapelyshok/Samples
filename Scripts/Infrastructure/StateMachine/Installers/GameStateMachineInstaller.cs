using AtomicApps.Infrastructure.StateMachine;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.StateMachine
{
    public class GameStateMachineInstaller : MonoInstaller
    {
        [SerializeField]
        private GameStateMachine gameStateMachine;

        public override void InstallBindings()
        {
            Container.Bind<GameStateMachine>().FromInstance(gameStateMachine).AsSingle().NonLazy();
        }
    }
}