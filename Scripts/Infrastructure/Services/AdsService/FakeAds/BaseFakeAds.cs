using UnityEngine;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public abstract class BaseFakeAds : MonoBehaviour
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}