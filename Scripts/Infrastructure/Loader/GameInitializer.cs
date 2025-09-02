using System;
using System.Collections;
using System.Collections.Generic;
using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Infrastructure.Services.Audio;
using AtomicApps.Infrastructure.Services.SaveLoad;
using AtomicApps.Infrastructure.Services.SaveLoad.Data;
using AtomicApps.Mechanics.Gameplay.Dictionary;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField]
        private Slider loaderSlider;
        [SerializeField]
        private TextMeshProUGUI loaderSliderText;
        [SerializeField]
        private float loadingTimeInSeconds = 6f;

        private GameStateMachine _stateMachine;
        private IWordsDictionaryService _wordsDictionaryService;
        private bool _isLoading;
        private ISaveService _saveService;
        private GameSaveData _saveData;

        [Inject]
        private void Construct(GameStateMachine stateMachine, IWordsDictionaryService wordsDictionaryService, ISaveService saveService)
        {
            _saveService = saveService;
            _wordsDictionaryService = wordsDictionaryService;
            _stateMachine = stateMachine;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            loadingTimeInSeconds = 0.1f;
#endif
            _saveData = _saveService.GetData<GameSaveData>();
            Initialize();
        }

        private async UniTask Initialize()
        {
            loaderSlider.value = 0f;
            if (loaderSliderText) loaderSliderText.text = "0%";

            Tween tween = loaderSlider
                .DOValue(1f, loadingTimeInSeconds)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    if (loaderSliderText)
                    {
                        int pct = Mathf.Clamp(Mathf.RoundToInt(loaderSlider.value * 100f), 0, 100);
                        loaderSliderText.text = pct + "%";
                    }
                })
                .SetUpdate(true)           
                .SetLink(gameObject);       

            var initTask = _wordsDictionaryService.Initialize();
            var tweenTask = tween.AsyncWaitForCompletion().AsUniTask();

            await UniTask.WhenAll(initTask, tweenTask);

            if (tween.IsActive()) tween.Kill();
            loaderSlider.value = 1f;
            if (loaderSliderText) loaderSliderText.text = "100%";
            if (_saveData.IsFirstLaunch)
            {
                _saveData.IsFirstLaunch = false;
                _saveService.SaveDataImmediately(_saveData);
                _stateMachine.Enter<LoadGameplayState, bool>(false);
            }
            else
            {
                _stateMachine.Enter<LoadLobbyState, bool>(false);
            }
        }
    }

    [Serializable]
    public class GameSaveData : SavableData
    {
        public bool IsFirstLaunch = true;
        
        public override void SetDefaultValues(object inData = null)
        {
            IsFirstLaunch = true;
        }
    }
}
