using System;
using AtomicApps.Infrastructure.Services.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps.UI.Mechanics
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonWrapper : MonoBehaviour
    {
        [SerializeField]
        private bool isAffectedByButtonsBlocker = true;
        [SerializeField]
        private bool isChangingVisualsOnBlock = true;
        
        protected Button _button;
        protected CanvasGroup _canvasGroup;

        protected IAudioService _audioService;
        
        public event Action OnClicked;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        protected virtual void OnEnable()
        {
            _button.onClick.AddListener(ClickLogicInvoke);
            
            if (isAffectedByButtonsBlocker)
            {
                ButtonsBlockerService.OnButtonsBlockedStateChanged += ChangeInteractable;
            }
        }

        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(ClickLogicInvoke);
            
            if (isAffectedByButtonsBlocker)
            {
                ButtonsBlockerService.OnButtonsBlockedStateChanged -= ChangeInteractable;
            }
        }

        public void ClickLogicInvoke()
        {
            OnClickHandler();
            OnClicked?.Invoke();
        }

        public bool IsInteractable() => 
            _button.interactable;
        
        public void ChangeInteractable(bool value)
        {
            if (_button != null)
            {
                _button.interactable = value;
                if (isChangingVisualsOnBlock)
                {
                    _canvasGroup.alpha = value ? 1f : .7f;
                }
            }
        }       
        
        public void ChangeInteractableWithoutAffectingVisuals(bool value)
        {
            if (_button != null)
            {
                _button.interactable = value;
            }
        }

        public abstract void OnClickHandler();
    }
}
