using Sirenix.OdinInspector;
using UnityEngine;

namespace HungNT.Advertisement.Demo
{
    /// <summary>
    /// Demo cách sử dụng AdsService thông qua ServiceLocator.
    /// Gán vào một GameObject, chạy Play Mode, nhấn các nút trong Inspector.
    /// </summary>
    public class AdsServiceDemo : MonoBehaviour
    {
        private IAdsService _adsService;

        private void Start()
        {
            this.GetService<IAdsService>(service => _adsService = service);
        }

        // ── Banner ───────────────────────────────────────────────────────────

        [Button]
        public void ShowBanner()
        {
            _adsService.ShowBanner();
            this.Log("ShowBanner called.");
        }

        [Button]
        public void HideBanner()
        {
            _adsService.HideBanner();
            this.Log("HideBanner called.");
        }

        // ── App Open ─────────────────────────────────────────────────────────

        [Button]
        public void ShowAppOpen()
        {
            _adsService.ShowAppOpen(onComplete: () =>
            {
                this.Log("AppOpen completed.");
            });
        }

        // ── Interstitial ─────────────────────────────────────────────────────

        [Button]
        public void CheckInterstitialReady()
        {
            this.Log($"IsInterstitialReady: {_adsService.IsInterstitialReady()}");
        }

        [Button]
        public void ShowInterstitialDefault()
        {
            _adsService.ShowInterstitial(
                placement: AdsPlacement.DEFAULT
                // onSuccess: () => this.Log("Interstitial — success."),
                // onFailure: () => this.Log("Interstitial — failure (cooldown / not ready).")
            );
        }

        [Button]
        public void ShowInterstitialLevelComplete()
        {
            _adsService.ShowInterstitial(
                placement: AdsPlacement.LEVEL_COMPLETE
                // onSuccess: () => this.Log("Interstitial level_complete — success."),
                // onFailure: () => this.Log("Interstitial level_complete — failure.")
            );
        }

        // ── Rewarded ─────────────────────────────────────────────────────────

        [Button]
        public void CheckRewardedReady()
        {
            this.Log($"IsRewardedReady: {_adsService.IsRewardedReady()}");
        }

        [Button]
        public void ShowRewardedDoubleCoin()
        {
            _adsService.ShowRewarded(
                placement: AdsPlacement.DOUBLE_COIN,
                onSuccess: OnGetRewardSuccess,
                onFailure: OnGetRewardFailure
            );
        }

        [Button]
        public void ShowRewardedExtraLife()
        {
            _adsService.ShowRewarded(
                placement: AdsPlacement.EXTRA_LIFE,
                onSuccess: OnGetRewardSuccess,
                onFailure: OnGetRewardFailure
            );
        }

        private void OnGetRewardSuccess()
        {
            this.Log("Get reward ads success!".Color("lime"));
        }

        private void OnGetRewardFailure()
        {
            this.Log("Get reward ads failure!".Color("red"));
        }
    }
}