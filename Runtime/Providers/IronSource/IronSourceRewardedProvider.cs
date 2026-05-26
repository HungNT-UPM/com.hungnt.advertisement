#if USE_IRONSOURCE
using System;

namespace HungNT.Advertisement
{
    public class IronSourceRewardedProvider : IRewardedAdProvider
    {
        private bool _hasReceivedReward;

        private Action _pendingOnSuccess;
        private Action _pendingOnFailure;

        public void Initialize(AdsConfig config)
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnAdRewarded;
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnAdClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnAdShowFailed;

            LoadAd();
            this.Log("Rewarded initialized.");
        }

        public void Dispose()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnAdRewarded;
            IronSourceRewardedVideoEvents.onAdClosedEvent -= OnAdClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent -= OnAdShowFailed;
        }

        public void LoadAd()
        {
            // IronSource auto-loads rewarded videos
        }

        public bool IsAdReady()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
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
                return;
            }

            IronSource.Agent.showRewardedVideo();
        }

        // ── IronSource Callbacks ─────────────────────────────────────────────

        private void OnAdRewarded(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            _hasReceivedReward = true;
        }

        private void OnAdClosed(IronSourceAdInfo adInfo)
        {
            if (_hasReceivedReward)
                InvokeSuccess();
            else
                InvokeFailure();
        }

        private void OnAdShowFailed(IronSourceError error, IronSourceAdInfo adInfo)
        {
            this.LogWarning($"Rewarded show failed: {error}");
            InvokeFailure();
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
