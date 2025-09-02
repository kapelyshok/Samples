using System;
using AtomicApps.UI.Popups;
using Cysharp.Threading.Tasks;
using static AtomicApps.UIConstants;

namespace AtomicApps.Infrastructure.Services.Popups.Interfaces
{
    public interface IPopupService
    {
        public UniTask<BasePopup> ShowPopupAsync(string popupId, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver, params object[] data);
        public UniTask<TPopup> ShowPopupAsync<TPopup>(string popupId, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver, params object[] data) where TPopup : BasePopup;
        public void CloseAllPopups();
        public void CloseCurrentPopup();
        public void ClosePopup(string type);
        public int GetOpenPopupsCount();
        public BasePopup GetCurrentPopup();
        public event Action OnAllPopupsClosed;
        public event Action<BasePopup> OnPopupOpened;
    }
}