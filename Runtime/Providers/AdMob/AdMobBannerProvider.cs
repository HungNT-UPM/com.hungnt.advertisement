#if USE_ADMOB
using GoogleMobileAds.Api;

namespace HungNT.Advertisement
{
    /// <summary>
    /// AdMob Banner provider sử dụng Google Mobile Ads SDK.
    /// </summary>
    public class AdMobBannerProvider : IBannerAdProvider
    {
        private BannerView _bannerView;
        private string _adUnitId;
        private bool _isBannerShowing;

        public void Initialize(AdsConfig config)
        {
            _adUnitId = config.AdMobBannerId;
            this.Log("Banner initialized.");
        }

        public void Dispose()
        {
            _bannerView?.Destroy();
            _bannerView = null;
        }

        public void ShowBanner()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;

            if (_bannerView != null)
            {
                _bannerView.Show();
                _isBannerShowing = true;
                return;
            }

            _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

            _bannerView.OnBannerAdLoaded += () =>
            {
                this.Log("Banner ad loaded.");
            };

            _bannerView.OnBannerAdLoadFailed += error =>
            {
                this.LogWarning($"Banner load failed: {error}");
            };

            var adRequest = new AdRequest();
            _bannerView.LoadAd(adRequest);
            _isBannerShowing = true;
        }

        public void HideBanner()
        {
            _bannerView?.Hide();
            _isBannerShowing = false;
        }
    }
}
#endif
