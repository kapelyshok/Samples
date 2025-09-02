using System;
using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.Levels;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Lobby
{
    public class LobbyLevelsManager : MonoBehaviour
    {
        [SerializeField]
        private List<LobbyLevelItemView> lobbyLevelItemViews;
        
        private ILevelSelectorService _levelSelectorService;

        [Inject]
        private void Construct(ILevelSelectorService levelSelectorService)
        {
            _levelSelectorService = levelSelectorService;
        }

        private void Awake()
        {
            int firstIndex = _levelSelectorService.GetCurrentLevelIndex();

            foreach (var views in lobbyLevelItemViews)
            {
                views.Init(firstIndex + 1);
                firstIndex++;
            }
        }
    }
}
