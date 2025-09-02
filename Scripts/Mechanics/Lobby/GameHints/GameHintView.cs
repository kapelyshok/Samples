using System;
using System.Collections;
using System.Collections.Generic;
using AtomicApps.Mechanics.Lobby.GameHints;
using TMPro;
using UnityEngine;
using Zenject;

namespace AtomicApps
{
    public class GameHintView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;
        
        private IGameHintsService _gameHintsService;

        [Inject]
        private void Construct(IGameHintsService gameHintsService)
        {
            _gameHintsService = gameHintsService;
        }

        private void Awake()
        {
            text.text = _gameHintsService.GetRandomHintMapping().GameHint;
        }
    }
}
