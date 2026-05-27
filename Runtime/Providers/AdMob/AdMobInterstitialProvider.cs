#if USE_ADMOB
using System;
using GoogleMobileAds.Api;

namespace HungNT.Advertisement
{
    /// <summary>
    /// AdMob Interstitial provider sử dụng Google Mobile Ads SDK.
    /// </summary>
    public class AdMobInterstitialProvider : IInterstitialAdProvider
    {
        private InterstitialAd _interstitialAd;
        private string _adUnitId;

        private Action _pendingOnSuccess;
        private Action _pendingOnFailure;

        public void Initialize(AdsConfig config)
        {
            _adUnitId = config.AdMobInterId;
            LoadAd();
            this.Log("Interstitial initialized.");
        }

        public void Dispose()
        {
            _interstitialAd?.Destroy();
            _interstitialAd = null;
        }

        public void LoadAd()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;

            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            var adRequest = new AdRequest();
            InterstitialAd.Load(_adUnitId, adRequest, (ad, error) =>
            {
                if (error != null)
                {
                    this.LogWarning($"Interstitial load failed: {error}");
                    return;
                }

                _interstitialAd = ad;
                RegisterAdEvents();
                this.Log("Interstitial ad loaded.");
            });
        }

        public bool IsAdReady()
        {
            return _interstitialAd != null && _interstitialAd.CanShowAd();
        }

        public void ShowAd(Action onSuccess, Action onFailure)
        {
            _pendingOnSuccess = onSuccess;
            _pendingOnFailure = onFailure;

            if (!IsAdReady())
            {
                this.LogWarning("Interstitial not ready.");
                InvokeFailure();
                LoadAd();
                return;
            }

            _interstitialAd.Show();
        }

        // ── Ad Events ─────────────────────────────────────────────────────────

        private void RegisterAdEvents()
        {
            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                InvokeSuccess();
                LoadAd();
            };

            _interstitialAd.OnAdFullScreenContentFailed += error =>
            {
                this.LogWarning($"Interstitial show failed: {error}");
                InvokeFailure();
                LoadAd();
            };
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void InvokeSuccess()
        {
            var cb = _pendingOnSuccess;
            _pendingOnSuccess = null;
            _pendingOnFailure = null;
            cb?.Invoke();
        }

        private void InvokeFailure()
        {
            var cb = _pendingOnFailure;
            _pendingOnSuccess = null;
            _pendingOnFailure = null;
            cb?.Invoke();
        }
    }
}
#endif
