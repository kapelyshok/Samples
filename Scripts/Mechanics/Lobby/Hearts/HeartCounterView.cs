using System;
using System.Collections;
using System.Collections.Generic;
using AtomicApps.Infrastructure.Configs;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.Mechanics.Lobby.Hearts;
using AtomicApps.Scpts.Mechanics.Lobby.Hearts;
using AtomicApps.UI.Mechanics;
using AtomicApps.Utils;
using TMPro;
using UnityEngine;
using Zenject;

namespace AtomicApps
{
        public class HeartCounterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI heartCountText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private CustomButton plusButton;
        //[SerializeField] private GameObject normalHeart;
        //[SerializeField] private GameObject endlessHeart;
        [SerializeField] private GameObject timerParent;

        private IHearthService _hearthService;
        private IHeartRegenService _heartRegenService;
        private IPopupService _popupService;
        private GameConfigSO _gameConfigSo;

        [Inject]
        private void Constrcut(IHearthService hearthService, IHeartRegenService heartRegenService, IPopupService popupService, GameConfigSO gameConfigSo)
        {
            _gameConfigSo = gameConfigSo;
            _hearthService = hearthService;
            _heartRegenService = heartRegenService;
            _popupService = popupService;
            _hearthService.OnUpdated += OnUpdatedHandler; 
            OnUpdatedHandler();
        }

        private void OnEnable()
        {
            if (plusButton != null)
            {
                plusButton.OnClicked += OnPlusButtonClickedHandler;
            }
        }

        private void OnDisable()
        {
            if (plusButton != null)
            {
                plusButton.OnClicked -= OnPlusButtonClickedHandler;
            }
        }


        private void OnDestroy()
        {
            _hearthService.OnUpdated -= OnUpdatedHandler;
        }

        private void Update()
        {
            //plusButton.gameObject.SetActive(false);
            //endlessHeart.gameObject.SetActive(_hearthService.IsFree);
            //normalHeart.gameObject.SetActive(!_hearthService.IsFree);
            
            if (_hearthService.IsFree)
            {
                if(!timerParent.activeSelf) timerParent.SetActive(true);
                timerText.SetText(TimeSpan.FromSeconds(_hearthService.FreeTime).ToStringMMSS());
                return;
            }
            
            if (!_hearthService.IsMax)
            {
                if(!timerParent.activeSelf) timerParent.SetActive(true);
                timerText.SetText(_heartRegenService.LeftTimeToString);
                if (_hearthService.Count != 0) return;
                //plusButton.gameObject.SetActive(true);
                return;
            }

            if(timerParent.activeSelf) timerParent.SetActive(false);
            timerText.SetText($"{_gameConfigSo.MaxHearts}/{_gameConfigSo.MaxHearts}");
        }

        private void OnUpdatedHandler()
        {
            heartCountText.SetText($"{_hearthService.Count.ToString()}/{_gameConfigSo.MaxHearts}");
        }
        
        private void OnPlusButtonClickedHandler()
        {
            
        }
    }
}
