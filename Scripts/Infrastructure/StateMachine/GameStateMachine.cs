using System;
using System.Collections.Generic;
using AtomicApps.Infrastructure;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.StateMachine
{
    public class GameStateMachine : MonoBehaviour
    {
        private Dictionary<Type, IExitableState> _states;
        private IExitableState _currentState;
        private SceneLoaderService _sceneLoaderService;
        private IPopupService _popupService;
        private IAudioService _audioService;

        [Inject]
        private void Construct(SceneLoaderService sceneLoaderService, IPopupService popupService, IAudioService audioService)
        {
            _audioService = audioService;
            _popupService = popupService;
            _sceneLoaderService = sceneLoaderService;
        }

        private void Awake()
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, _sceneLoaderService,_audioService),
                [typeof(LoadLobbyState)] = new LoadLobbyState(this, _sceneLoaderService),
                [typeof(LobbyState)] = new LobbyState(this),
                [typeof(LoadGameplayState)] = new LoadGameplayState(this, _sceneLoaderService),
                [typeof(GameplayState)] = new GameplayState(this,_audioService),
                [typeof(PerksSelectionState)] = new PerksSelectionState(this, _popupService),
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
            state?.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            var state = ChangeState<TState>();
            state?.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _currentState?.Exit();
            TState state = GetState<TState>();
            _currentState = state;
            Debug.Log($"Entered state {_currentState.GetType().Name}");
            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            return _states[typeof(TState)] as TState;
        }
    }
}
