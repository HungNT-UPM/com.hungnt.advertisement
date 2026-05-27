#if USE_ADMOB
using System;
using GoogleMobileAds.Api;

namespace HungNT.Advertisement
{
    /// <summary>
    /// AdMob Rewarded provider sử dụng Google Mobile Ads SDK.
    /// </summary>
    public class AdMobRewardedProvider : IRewardedAdProvider
    {
        private RewardedAd _rewardedAd;
        private string _adUnitId;
        private bool _hasReceivedReward;

        private Action _pendingOnSuccess;
        private Action _pendingOnFailure;

        public void Initialize(AdsConfig config)
        {
            _adUnitId = config.AdMobRewardedId;
            LoadAd();
            this.Log("Rewarded initialized.");
        }

        public void Dispose()
        {
            _rewardedAd?.Destroy();
            _rewardedAd = null;
        }

        public void LoadAd()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;

            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = new AdRequest();
            RewardedAd.Load(_adUnitId, adRequest, (ad, error) =>
            {
                if (error != null)
                {
                    this.LogWarning($"Rewarded load failed: {error}");
                    return;
                }

                _rewardedAd = ad;
                RegisterAdEvents();
                this.Log("Rewarded ad loaded.");
            });
        }

        public bool IsAdReady()
        {
            return _rewardedAd != null && _rewardedAd.CanShowAd();
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

            _rewardedAd.Show(reward =>
            {
                _hasReceivedReward = true;
            });
        }

        // ── Ad Events ─────────────────────────────────────────────────────────

        private void RegisterAdEvents()
        {
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                if (_hasReceivedReward)
                    InvokeSuccess();
                else
                    InvokeFailure();

                LoadAd();
            };

            _rewardedAd.OnAdFullScreenContentFailed += error =>
            {
                this.LogWarning($"Rewarded show failed: {error}");
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
