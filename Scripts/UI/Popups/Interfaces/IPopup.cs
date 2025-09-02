using System;

namespace AtomicApps.UI.Popups
{
    public interface IPopup
    {
        public event Action<IPopup> OnPopupOpened;
        public event Action<IPopup> OnPopupClosed;
        public void Show(object[] inData = null);
        public void Close();
    }
}
