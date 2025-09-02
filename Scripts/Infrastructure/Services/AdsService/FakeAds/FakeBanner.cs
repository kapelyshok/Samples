using UnityEngine;

namespace AtomicApps.Infrastructure.Services.AdsService
{
    public class FakeBanner : BaseFakeAds
    {
        [SerializeField]
        private RectTransform bannerRect;

        public override void Show()
        {
            base.Show();
        }

        public float GetHeight()
        {
            return bannerRect.rect.height;
        }
    }
}