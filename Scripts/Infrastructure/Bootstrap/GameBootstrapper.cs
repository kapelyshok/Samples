using System;
using AtomicApps.Infrastructure.StateMachine;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public class GameBootstrapper : MonoBehaviour
    {
        private SceneLoaderService _sceneLoaderService;
        private GameStateMachine _stateMachine;

        [Inject]
        private void Construct(SceneLoaderService sceneLoaderService, GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _sceneLoaderService = sceneLoaderService;
        }

        private void Awake()
        {
            _stateMachine.Enter<BootstrapState>();
            
            DontDestroyOnLoad(this);
        }
    }
}
