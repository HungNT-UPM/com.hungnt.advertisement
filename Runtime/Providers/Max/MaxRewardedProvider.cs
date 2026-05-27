#if USE_MAX
using System;

namespace HungNT.Advertisement
{
    public class MaxRewardedProvider : IRewardedAdProvider
    {
        private string _adUnitId;
        private int _retryAttempt;
        private bool _hasReceivedReward;

        private Action _pendingOnSuccess;
        private Action _pendingOnFailure;

        public void Initialize(AdsConfig config)
        {
            _adUnitId = config.MaxRewardedId;

            if (string.IsNullOrEmpty(_adUnitId)) return;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedReward;

            LoadAd();
            this.Log("Rewarded initialized.");
        }

        public void Dispose()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedReward;
        }

        public void LoadAd()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;
            MaxSdk.LoadRewardedAd(_adUnitId);
        }

        public bool IsAdReady()
        {
            return !string.IsNullOrEmpty(_adUnitId) && MaxSdk.IsRewardedAdReady(_adUnitId);
        }

        public void ShowAd(Action onSuccess, Action onFailure)
        {
            _pendingOnSuccess = onSuccess;
            _pendingOnFailure = onFailure;
            _hasReceivedReward = false;

            if (!IsAdReady())
            {
                this.LogWarning("Rewarded not ready.");
                InvokeFailure();
                LoadAd();
                return;
            }

            MaxSdk.ShowRewardedAd(_adUnitId);
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
            this.LogWarning($"Rewarded load failed, retry in {retryDelay}s");
        }

        private void OnAdReceivedReward(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            _hasReceivedReward = true;
        }

        private void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (_hasReceivedReward)
                InvokeSuccess();
            else
                InvokeFailure();

            LoadAd();
        }

        private void OnAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            this.LogWarning($"Rewarded display failed: {errorInfo}");
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
