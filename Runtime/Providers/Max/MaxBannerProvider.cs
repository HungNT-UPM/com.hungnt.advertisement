#if USE_MAX
using UnityEngine;

namespace HungNT.Advertisement
{
    public class MaxBannerProvider : IBannerAdProvider
    {
        private string _adUnitId;
        private bool _isBannerShowing;

        public void Initialize(AdsConfig config)
        {
            _adUnitId = config.MaxBannerId;

            if (string.IsNullOrEmpty(_adUnitId)) return;

            MaxSdk.CreateBanner(_adUnitId, new MaxSdkBase.AdViewConfiguration(MaxSdkBase.AdViewPosition.BottomCenter));
            MaxSdk.SetBannerBackgroundColor(_adUnitId, Color.black);

            this.Log("Banner initialized.");
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(_adUnitId))
                MaxSdk.DestroyBanner(_adUnitId);
        }

        public void ShowBanner()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;

            MaxSdk.ShowBanner(_adUnitId);
            _isBannerShowing = true;
        }

        public void HideBanner()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;

            MaxSdk.HideBanner(_adUnitId);
            _isBannerShowing = false;
        }
    }
}
#endif
