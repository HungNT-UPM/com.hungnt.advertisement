using System;

namespace HungNT.Advertisement
{
    /// <summary>
    /// Null Object implementation của <see cref="IAdsService"/>.
    /// Dùng làm fallback khi chưa có SDK thật, hoặc trong Editor/test.
    /// Mọi hàm show đều no-op và gọi onSuccess.
    /// </summary>
    public class NullAdsService : IAdsService
    {
        public void Initialize()
        {
        }

        public void LateInitialize()
        {
        }

        public void ShowBanner()
        {
        }

        public void HideBanner()
        {
        }

        public void ShowAppOpen(Action onComplete = null)
        {
            onComplete?.Invoke();
        }

        public bool IsInterstitialReady()
        {
            return false;
        }

        public void ShowInterstitial(
            string placement = AdsPlacement.DEFAULT,
            Action onSuccess = null,
            Action onFailure = null)
        {
            onSuccess?.Invoke();
        }

        public bool IsRewardedReady()
        {
            return false;
        }

        public void ShowRewarded(
            string placement,
            Action onSuccess,
            Action onFailure = null)
        {
            onSuccess?.Invoke();
        }
    }
}