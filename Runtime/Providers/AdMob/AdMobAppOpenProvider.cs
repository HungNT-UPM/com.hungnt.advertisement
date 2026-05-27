#if USE_ADMOB
using System;
using GoogleMobileAds.Api;

namespace HungNT.Advertisement
{
    public class AdMobAppOpenProvider : IAppOpenAdProvider
    {
        private AppOpenAd _appOpenAd;
        private string _adUnitId;
        private bool _isShowingAd;

        public void Initialize(AdsConfig config)
        {
            _adUnitId = config.AdMobAppOpenId;
            LoadAd();
            this.Log("AppOpen initialized.");
        }

        public void Dispose()
        {
            _appOpenAd?.Destroy();
            _appOpenAd = null;
        }

        public void LoadAd()
        {
            if (_appOpenAd != null)
            {
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }

            var adRequest = new AdRequest();
            AppOpenAd.Load(_adUnitId, adRequest, (ad, error) =>
            {
                if (error != null)
                {
                    this.LogWarning($"AppOpen load failed: {error}");
                    return;
                }

                _appOpenAd = ad;
                this.Log("AppOpen ad loaded.");
            });
        }

        public bool IsAdReady()
        {
            return _appOpenAd != null && _appOpenAd.CanShowAd();
        }

        public void ShowAd(Action onComplete)
        {
            if (!IsAdReady())
            {
                this.LogWarning("AppOpen ad not ready.");
                onComplete?.Invoke();
                LoadAd();
                return;
            }

            _appOpenAd.OnAdFullScreenContentClosed += () =>
            {
                _isShowingAd = false;
                onComplete?.Invoke();
                LoadAd();
            };

            _appOpenAd.OnAdFullScreenContentFailed += error =>
            {
                this.LogWarning($"AppOpen show failed: {error}");
                _isShowingAd = false;
                onComplete?.Invoke();
                LoadAd();
            };

            _isShowingAd = true;
            _appOpenAd.Show();
        }
    }
}
#endif
