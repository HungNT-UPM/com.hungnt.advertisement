#if USE_IRONSOURCE
using System;

namespace HungNT.Advertisement
{
    public class IronSourceInterstitialProvider : IInterstitialAdProvider
    {
        private Action _pendingOnSuccess;
        private Action _pendingOnFailure;

        public void Initialize(AdsConfig config)
        {
            IronSourceInterstitialEvents.onAdShowSucceededEvent += OnAdShowSucceeded;
            IronSourceInterstitialEvents.onAdShowFailedEvent += OnAdShowFailed;
            IronSourceInterstitialEvents.onAdClosedEvent += OnAdClosed;

            LoadAd();
            this.Log("Interstitial initialized.");
        }

        public void Dispose()
        {
            IronSourceInterstitialEvents.onAdShowSucceededEvent -= OnAdShowSucceeded;
            IronSourceInterstitialEvents.onAdShowFailedEvent -= OnAdShowFailed;
            IronSourceInterstitialEvents.onAdClosedEvent -= OnAdClosed;
        }

        public void LoadAd()
        {
            IronSource.Agent.loadInterstitial();
        }

        public bool IsAdReady()
        {
            return IronSource.Agent.isInterstitialReady();
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

            IronSource.Agent.showInterstitial();
        }

        // ── IronSource Callbacks ─────────────────────────────────────────────

        private void OnAdShowSucceeded(IronSourceAdInfo adInfo) { }

        private void OnAdShowFailed(IronSourceError error, IronSourceAdInfo adInfo)
        {
            this.LogWarning($"Interstitial show failed: {error}");
            InvokeFailure();
            LoadAd();
        }

        private void OnAdClosed(IronSourceAdInfo adInfo)
        {
            InvokeSuccess();
            LoadAd();
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