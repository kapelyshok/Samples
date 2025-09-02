using System;
using AtomicApps.Infrastructure.Services.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Zenject;
using static AtomicApps.UIConstants;

namespace AtomicApps.UI.Popups
{
    public class BasePopup : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField]
        protected CanvasGroup backgroundDim;
        [SerializeField]
        protected RectTransform content;
        [SerializeField]
        protected UIAnimation inAnimation;
        [SerializeField]
        protected UIAnimation outAnimation;

        [Space]
        
        protected string _id;
        private bool _isPerformingAnimation = false;
        private float _dimAlpha;

        public event Action<BasePopup> OnPopupOpened;
        public event Action<BasePopup> OnPopupClosed;
        
        public void Init(string id)
        {
            _id = id;
        }

        public string GetID() => _id;

        public virtual void Show(object[] inData = null)
        {
            _dimAlpha = backgroundDim.alpha;
            
            if (_isPerformingAnimation)
            {
                Debug.LogWarning("Haven't finished previous animation yet!");
                return;
            }
            
            if (inAnimation != null)
            {
                _isPerformingAnimation = true;
                inAnimation.ActivateAsync(content.gameObject, true, () =>
                {
                    _isPerformingAnimation = false;
                    OnPopupOpened?.Invoke(this);
                });
                if (backgroundDim != null)
                {
                    backgroundDim.alpha = 0f;
                    backgroundDim.DOFade(_dimAlpha, 0.5f);
                }
            }
            else
            {
                gameObject.SetActive(true);
                OnPopupOpened?.Invoke(this);
            }
        }
        
        private void OnDestroy()
        {
            Close();   
        }

        public virtual void Close()
        {
            if (_isPerformingAnimation)
            {
                Debug.LogWarning("Haven't finished previous animation yet!");
                return;
            }
            
            if (outAnimation != null)
            {
                _isPerformingAnimation = true;
                outAnimation.ActivateAsync(content.gameObject,false, () =>
                {
                    _isPerformingAnimation = false;
                    gameObject.SetActive(false);
                    DestroyPopup();
                });
                if (backgroundDim != null)
                {
                    backgroundDim.alpha = _dimAlpha;
                    backgroundDim.DOFade(0, 0.5f);
                }
            }
            else
            {
                gameObject.SetActive(false);
                DestroyPopup();
            }
        }
        
        public virtual void DestroyPopup()
        {
            if (gameObject == null)
            {
                Debug.LogError("Failed to destroy the popup because it no longer exists", gameObject);
                return;
            }

            if (this != null)
            {
                OnPopupClosed?.Invoke(this);
            }
            else
            {
                Debug.LogError("Popup: DestroyPopup : 'this' is null", gameObject);
            }
        }

        public virtual void UnHide()
        {
            gameObject.SetActive(true);
            Debug.Log("Popup: UnHide : " + _id, gameObject);
        }

        public virtual void TemporarilyHide()
        {
            gameObject.SetActive(false);
            Debug.Log("Popup: Temporarily Hide : " + _id, gameObject);
        }
    }
}
