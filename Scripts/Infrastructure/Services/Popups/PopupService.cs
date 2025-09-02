using System;
using System.Collections.Generic;
using AtomicApps.Infrastructure.Services.Popups.AssetsLoaders;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.Tools;
using AtomicApps.UI.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static AtomicApps.UIConstants;

namespace AtomicApps.Infrastructure.Services.Popups
{
    public class PopupService : MonoBehaviour, IPopupService
    {
        [HelpBox(
            @"SETUP INSTRUCTION
1. Assign Addressable or InScene realisation of IPopupAssetsLoader to popupAssetsLoaderObject.
2. Follow setup instruction of selected assets loader option.
3. Popup can be shown by ShowPopup method with id from PopupKeys.", HelpBoxMessageType.Info)]
        [Space]
        
        [SerializeField]
        private GameObject popupAssetsLoaderObject;
        
        [Inject] private readonly SignalBus _signalBus;

        private IPopupAssetsLoader _popupAssetsLoader;
        
        private BasePopup _current;
        
        private List<BasePopup> _popups = new ();

        public event Action OnAllPopupsClosed;
        public event Action<BasePopup> OnPopupOpened;
        
        private void Awake()
        {
            if (popupAssetsLoaderObject == null)
            {
                Debug.LogError($"PopupService initialization error! Asset loader is null");
            }
            
            _popupAssetsLoader = popupAssetsLoaderObject.GetComponent<IPopupAssetsLoader>();
 
            if (_popupAssetsLoader == null)
            {
                Debug.LogError($"PopupService initialization error! Attached asset loader doesn't have any suitable provider");
            }
            
            _signalBus.Subscribe<SceneContextReadySignal>(TryUpdateDiContainerForAssetLoader);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<SceneContextReadySignal>(TryUpdateDiContainerForAssetLoader);
        }
        
        private void TryUpdateDiContainerForAssetLoader(SceneContextReadySignal signal)
        {
            SceneContext currentSceneContext = signal.SceneContext;
            
            if (currentSceneContext != null)
            {
                _popupAssetsLoader.UpdateDiContainer(currentSceneContext.Container);
            }
            else
            {
                Debug.LogWarning("No SceneContext found in the loaded scene.");
            }
        }

        public async UniTask<BasePopup> ShowPopupAsync(string popupId, 
            UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver, params object[] data)
        {
            Debug.Log($"Try show popup: {popupId}");

            var popup = await _popupAssetsLoader.LoadPopupAsync<BasePopup>(popupId, option);
            ShowPopup<BasePopup>(popup,option,data);
            OnPopupOpened?.Invoke(popup);
            return popup;
        }
        
        public async UniTask<TPopup> ShowPopupAsync<TPopup>(string popupId, 
            UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver, params object[] data) where TPopup : BasePopup
        {
            Debug.Log($"Try show popup: {popupId}");
            
            var popup = await _popupAssetsLoader.LoadPopupAsync<TPopup>(popupId, option);
            ShowPopup<TPopup>(popup,option,data);
            OnPopupOpened?.Invoke(popup);
            return popup;
        }

        public void CloseAllPopups()
        {
            Debug.Log("PopupManager: CloseAllPopups", gameObject);
            
            if (_popups != null && _popups.Count > 0)
            {
                for (int i = _popups.Count-1; i >= 0; i--)
                {
                    _popupAssetsLoader.DisposePopup(_popups[i]);
                }
                _popups = new List<BasePopup>();
                _current = null;
                OnAllPopupsClosed?.Invoke();
            }
            else
            {
                Debug.Log("PopupManager: No open popups", gameObject);
            }
        }

        public void CloseCurrentPopup()
        {
            Debug.Log("PopupManager: CloseCurrentPopup", gameObject);
            if (_current != null)
            {
                _current.Close();
            }
            else
            {
                Debug.LogError("PopupManager: CloseCurrentPopup > current is null", gameObject);
            }
        }

        public void ClosePopup(string type)
        {
            if (_current == null) return;
            
            if (_current.GetID() == type)
            {
                _current.Close();
            }
        }

        public int GetOpenPopupsCount()
        {
            return _popups.Count;
        }
        
        public BasePopup GetCurrentPopup()
        {
            return _current;
        }

        private TPopup ShowPopup<TPopup>(TPopup popup, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver, params object[] data) where TPopup : BasePopup
        {
            if (popup == null)
            {
                Debug.LogError("PopupManager: Try to show null popup!");
                return null;
            }

            if (popup == _current)
            {
                Debug.LogError("PopupManager: Try to show current popup!");
                return null;
            }

            if (_current != null && option == UIConstants.PopupShow.DontShowIfOthersShowing)
            {
                _popupAssetsLoader.DisposePopup(popup);
                return null;
            }

            BasePopup previous = _current;
            
            if (option != UIConstants.PopupShow.Queue || _popups.Count == 0)
            {            
                _current = popup;
            }

            if (option == UIConstants.PopupShow.Queue)
            {
                _popups.Insert(0,popup);
            }
            else if (option != UIConstants.PopupShow.ShowPrevious)
            {
                _popups.Add(popup);
            }

            if (previous != null)
            {
                switch (option)
                {
                    case UIConstants.PopupShow.ReplaceCurrent:
                        previous.Close();
                        break;
                    case UIConstants.PopupShow.Stack:
                        previous.TemporarilyHide();
                        break;
                    case UIConstants.PopupShow.Queue:
                        popup.Show(data);
                        popup.TemporarilyHide();
                        popup.OnPopupClosed += OnOnePopupClosedHandler;
                        break;
                }
            }

            if (option != UIConstants.PopupShow.ShowPrevious && option != UIConstants.PopupShow.Queue)
            {
                _current.OnPopupClosed += OnOnePopupClosedHandler;
            }

            if (_current != null)
            {
                if (option == UIConstants.PopupShow.ShowPrevious)
                {
                    _current.UnHide();
                    OnPopupOpened?.Invoke(popup);
                }
                else if(option == UIConstants.PopupShow.Queue && previous == null)
                { 
                    popup.Show(data);
                    OnPopupOpened?.Invoke(popup);
                }
                else if(option != UIConstants.PopupShow.Queue)
                {
                    _current.Show(data);
                    OnPopupOpened?.Invoke(popup);
                }
            }
            
            return popup;
        }

        private void OnOnePopupClosedHandler(BasePopup basePopup)
        {
            basePopup.OnPopupClosed -= OnOnePopupClosedHandler;
            
            //if (_current == basePopup)
            {
                _current = null;
                _popups.Remove(basePopup);

                if (_popups.Count > 0)
                {
                    BasePopup nextBasePopup = null;
                    while (nextBasePopup == null && _popups.Count > 0)
                    {
                        nextBasePopup = _popups[^1];
                        if (nextBasePopup == null)
                            _popups.RemoveAt(_popups.Count - 1);
                    }

                    if (nextBasePopup != null/* && basePopup != nextBasePopup*/)
                    {
                        ShowPopup(nextBasePopup, UIConstants.PopupShow.ShowPrevious);
                    }
                }
                
                if (OnAllPopupsClosed != null && _popups.Count == 0)
                {
                    OnAllPopupsClosed?.Invoke();
                }
            }
            /*else
            {
                _popups.Remove(basePopup);
                if (OnAllPopupsClosed != null && _popups.Count == 0)
                    OnAllPopupsClosed?.Invoke();
            }*/
            
            _popupAssetsLoader.DisposePopup(basePopup);
        }
    }
}
