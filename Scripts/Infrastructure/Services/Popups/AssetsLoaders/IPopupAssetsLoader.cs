using AtomicApps.UI.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static AtomicApps.UIConstants;


namespace AtomicApps.Infrastructure.Services.Popups.AssetsLoaders
{
    public interface IPopupAssetsLoader
    {
        public UniTask<TAssetType> LoadPopupAsync<TAssetType>(string popupId, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver) where TAssetType : BasePopup;
        public void DisposePopup(BasePopup basePopup);
        public void UpdateDiContainer(DiContainer newContainer);
    }
}