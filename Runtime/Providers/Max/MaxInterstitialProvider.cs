#if USE_MAX
using System;

namespace HungNT.Advertisement
{
    public class MaxInterstitialProvider : IInterstitialAdProvider
    {
        private string _adUnitId;
        private int _retryAttempt;

        private Action _pendingOnSuccess;
        private Action _pendingOnFailure;

        public void Initialize(AdsConfig config)
        {
            _adUnitId = config.MaxInterId;

            if (string.IsNullOrEmpty(_adUnitId)) return;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdHidden;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnAdDisplayFailed;

            LoadAd();
            this.Log("Interstitial initialized.");
        }

        public void Dispose()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnAdHidden;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnAdDisplayFailed;
        }

        public void LoadAd()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;
            MaxSdk.LoadInterstitial(_adUnitId);
        }

        public bool IsAdReady()
        {
            return !string.IsNullOrEmpty(_adUnitId) && MaxSdk.IsInterstitialReady(_adUnitId);
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

            MaxSdk.ShowInterstitial(_adUnitId);
        }

        // ── MAX Callbacks ────────────────────────────────────────────────────

        private void OnAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _retryAttempt = 0;
        }

        private void OnAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));
            this.LogWarning($"Interstitial load failed, retry in {retryDelay}s");
        }

        private void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            InvokeSuccess();
            LoadAd();
        }

        private void OnAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            this.LogWarning($"Interstitial display failed: {errorInfo}");
            InvokeFailure();
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
