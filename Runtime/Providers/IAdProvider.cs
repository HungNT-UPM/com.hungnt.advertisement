using System;
using HungNT;

namespace HungNT.Advertisement
{
    /// <summary>
    /// Base interface cho tất cả ad providers.
    /// </summary>
    public interface IAdProvider
    {
        void Initialize(AdsConfig config);
        void Dispose();
    }

    /// <summary>Provider cho App Open ads.</summary>
    public interface IAppOpenAdProvider : IAdProvider
    {
        void LoadAd();
        bool IsAdReady();
        void ShowAd(Action onComplete);
    }

    /// <summary>Provider cho Banner ads.</summary>
    public interface IBannerAdProvider : IAdProvider
    {
        void ShowBanner();
        void HideBanner();
    }

    /// <summary>Provider cho Interstitial ads.</summary>
    public interface IInterstitialAdProvider : IAdProvider
    {
        void LoadAd();
        bool IsAdReady();
        void ShowAd(Action onSuccess, Action onFailure);
    }

    /// <summary>Provider cho Rewarded ads.</summary>
    public interface IRewardedAdProvider : IAdProvider
    {
        void LoadAd();
        bool IsAdReady();
        void ShowAd(Action onSuccess, Action onFailure);
    }
}