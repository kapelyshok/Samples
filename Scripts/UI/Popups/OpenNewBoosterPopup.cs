using System.Collections.Generic;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Mechanics.Gameplay.FeaturesUnlocker;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers.Boosters;
using AtomicApps.UI.Mechanics;
using UnityEngine;
using Zenject;

namespace AtomicApps.UI.Popups
{
    public class OpenNewBoosterPopup : BasePopup
    {
        [SerializeField]
        private List<BoosterImageData> boosterImages;
        
        [SerializeField]
        private CustomButton confirmButton;

        private BoosterType _boosterType;
        private BoostersUnlockerManager _boostersUnlockerManager;
        private RectTransform _currentIcon;
        private IAudioService _audioService;

        [Inject]
        private void Construct(BoostersUnlockerManager boostersUnlockerManager, IAudioService audioService)
        {
            _audioService = audioService;
            _boostersUnlockerManager = boostersUnlockerManager;
        }

        public override void Show(object[] inData = null)
        {
            if (inData != null && inData.Length > 0)
            {
                if (inData[0] is BoosterType)
                {
                    _boosterType = (BoosterType)inData[0];
                    
                    foreach (var boosterImage in boosterImages)
                    {
                        boosterImage.Image.SetActive(boosterImage.BoosterType == _boosterType);

                        if (boosterImage.BoosterType == _boosterType)
                        {
                            _currentIcon = boosterImage.Image.GetComponent<RectTransform>();
                        }
                    }
                }
            }

            confirmButton.OnClicked += GiveStartBoosters;
            
            base.Show(inData);
        }

        public override void Close()
        {
            _audioService.PlaySound(SoundKeys.TAP_CLOSE);

            confirmButton.OnClicked -= GiveStartBoosters;
            
            base.Close();
        }

        private void GiveStartBoosters()
        {
            _boostersUnlockerManager.AnimateNewBoosterIcons(_boosterType, _currentIcon);
            Close();
        }
    }
}