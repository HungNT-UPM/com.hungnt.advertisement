using System;

namespace HungNT.Advertisement
{
    /// <summary>
    /// Interface cho Ads service.
    /// Implement bằng SDK cụ thể (AdMob, MAX, IronSource, ...).
    /// </summary>
    public interface IAdsService : IService
    {
        // ── Banner ───────────────────────────────────────────────────────────

        void ShowBanner();

        void HideBanner();

        // ── App Open ─────────────────────────────────────────────────────────

        void ShowAppOpen(Action onComplete = null);

        // ── Interstitial ─────────────────────────────────────────────────────

        bool IsInterstitialReady();

        void ShowInterstitial(string placement = AdsPlacement.DEFAULT,
            Action onSuccess = null,
            Action onFailure = null);

        // ── Rewarded ─────────────────────────────────────────────────────────

        bool IsRewardedReady();

        void ShowRewarded(
            string placement,
            Action onSuccess,
            Action onFailure = null);
    }
}