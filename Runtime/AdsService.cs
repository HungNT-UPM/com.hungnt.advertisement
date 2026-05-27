using System;
using UnityEngine;

namespace HungNT.Advertisement
{
    /// <summary>
    /// Facade quản lý toàn bộ ads. Delegate xuống các ad providers theo SDK.
    /// Xử lý cooldown giữa các lần show, skip ads, auto-preload.
    /// Provider cho từng loại ads được cấu hình qua <see cref="AdsConfig"/>.
    /// </summary>
    public class AdsService : MonoBehaviour, IAdsService
    {
        [SerializeField] private bool _isSkipAds;

        // ── Providers ────────────────────────────────────────────────────────

        private IAppOpenAdProvider _appOpenProvider = new NullAppOpenAdProvider();
        private IBannerAdProvider _bannerProvider = new NullBannerAdProvider();
        private IInterstitialAdProvider _interProvider = new NullInterstitialAdProvider();
        private IRewardedAdProvider _rewardedProvider = new NullRewardedAdProvider();

        // ── Config & Timing ─────────────────────────────────────────────────

        private AdsConfig _config;
        private float _lastInterTime = -999f;
        private float _lastRewardTime = -999f;

        // ══════════════════════════════════════════════════════════════════════
        //  IService
        // ══════════════════════════════════════════════════════════════════════

        public void Initialize()
        {
            _config = Resources.Load<AdsConfig>(AdsDefine.ADS_CONFIG_PATH);
            if (_config == null)
            {
                this.LogError("AdsConfig not found at " + $"Resources/{AdsDefine.ADS_CONFIG_PATH}".Bold());
                return;
            }

            InitializeSdks(_config, onComplete: () => InitializeProviders(_config));
        }

        public void LateInitialize()
        {
        }

        private void OnDestroy()
        {
            _appOpenProvider.Dispose();
            _bannerProvider.Dispose();
            _interProvider.Dispose();
            _rewardedProvider.Dispose();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  SDK Initialization (centralized)
        // ══════════════════════════════════════════════════════════════════════

        private void InitializeSdks(AdsConfig config, Action onComplete)
        {
            // MAX & IronSource init đồng bộ — gọi trước.
            InitializeMaxSdk(config);
            InitializeIronSourceSdk(config);

            // AdMob init bất đồng bộ — phải chờ callback xong rồi mới tạo providers.
#if USE_ADMOB
            if (config.UsesAdMob)
            {
                GoogleMobileAds.Api.MobileAds.Initialize(_ =>
                {
                    this.Log("AdMob SDK initialized.");
                    onComplete?.Invoke();
                });
                return;
            }
#endif
            onComplete?.Invoke();
        }

        // ReSharper disable once UnusedParameter.Local
        private void InitializeMaxSdk(AdsConfig config)
        {
#if USE_MAX
            if (!config.UsesMax) return;

            if (!string.IsNullOrEmpty(config.MaxSdkKey))
                MaxSdk.SetSdkKey(config.MaxSdkKey);

            MaxSdk.InitializeSdk();
            this.Log("MAX SDK initialized.");
#endif
        }

        // ReSharper disable once UnusedParameter.Local
        private void InitializeIronSourceSdk(AdsConfig config)
        {
#if USE_IRONSOURCE
            if (!config.UsesIronSource) return;

            var appKey = config.IronSourceAppKey;
            if (string.IsNullOrEmpty(appKey))
            {
                this.LogWarning("IronSource AppKey is empty. Skipping init.");
                return;
            }

            IronSource.Agent.init(appKey);
            this.Log("IronSource SDK initialized.");
#endif
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Provider Factory
        // ══════════════════════════════════════════════════════════════════════

        private void InitializeProviders(AdsConfig config)
        {
            _appOpenProvider = CreateAppOpenProvider(config.AppOpenProviderType);
            _bannerProvider = CreateBannerProvider(config.BannerProviderType);
            _interProvider = CreateInterstitialProvider(config.InterProviderType);
            _rewardedProvider = CreateRewardedProvider(config.RewardedProviderType);

            _appOpenProvider.Initialize(config);
            _bannerProvider.Initialize(config);
            _interProvider.Initialize(config);
            _rewardedProvider.Initialize(config);

            this.Log("AdsService initialized: ".Color("lime") +
                     $"AppOpen: {config.AppOpenProviderType.ToString().Bold()}, " +
                     $"Banner: {config.BannerProviderType.ToString().Bold()}, " +
                     $"Inter: {config.InterProviderType.ToString().Bold()}, " +
                     $"Rewarded: {config.RewardedProviderType.ToString().Bold()}");
        }

        private IAppOpenAdProvider CreateAppOpenProvider(AdProviderType type)
        {
            switch (type)
            {
                case AdProviderType.AdMob:
#if USE_ADMOB
                    return new AdMobAppOpenProvider();
#else
                    WarnMissingDefine(AdType.AppOpen, type);
                    return new NullAppOpenAdProvider();
#endif
                case AdProviderType.Max:
                case AdProviderType.IronSource:
                    this.LogWarning($"{AdType.AppOpen} provider {type} is not implemented. Falling back to Null.");
                    return new NullAppOpenAdProvider();
                default:
                    return new NullAppOpenAdProvider();
            }
        }

        private IBannerAdProvider CreateBannerProvider(AdProviderType type)
        {
            switch (type)
            {
                case AdProviderType.AdMob:
#if USE_ADMOB
                    return new AdMobBannerProvider();
#else
                    WarnMissingDefine(AdType.Banner, type);
                    return new NullBannerAdProvider();
#endif
                case AdProviderType.Max:
#if USE_MAX
                    return new MaxBannerProvider();
#else
                    WarnMissingDefine(AdType.Banner, type);
                    return new NullBannerAdProvider();
#endif
                case AdProviderType.IronSource:
#if USE_IRONSOURCE
                    return new IronSourceBannerProvider();
#else
                    WarnMissingDefine(AdType.Banner, type);
                    return new NullBannerAdProvider();
#endif
                default:
                    return new NullBannerAdProvider();
            }
        }

        private IInterstitialAdProvider CreateInterstitialProvider(AdProviderType type)
        {
            switch (type)
            {
                case AdProviderType.AdMob:
#if USE_ADMOB
                    return new AdMobInterstitialProvider();
#else
                    WarnMissingDefine(AdType.Interstitial, type);
                    return new NullInterstitialAdProvider();
#endif
                case AdProviderType.Max:
#if USE_MAX
                    return new MaxInterstitialProvider();
#else
                    WarnMissingDefine(AdType.Interstitial, type);
                    return new NullInterstitialAdProvider();
#endif
                case AdProviderType.IronSource:
#if USE_IRONSOURCE
                    return new IronSourceInterstitialProvider();
#else
                    WarnMissingDefine(AdType.Interstitial, type);
                    return new NullInterstitialAdProvider();
#endif
                default:
                    return new NullInterstitialAdProvider();
            }
        }

        private IRewardedAdProvider CreateRewardedProvider(AdProviderType type)
        {
            switch (type)
            {
                case AdProviderType.AdMob:
#if USE_ADMOB
                    return new AdMobRewardedProvider();
#else
                    WarnMissingDefine(AdType.Rewarded, type);
                    return new NullRewardedAdProvider();
#endif
                case AdProviderType.Max:
#if USE_MAX
                    return new MaxRewardedProvider();
#else
                    WarnMissingDefine(AdType.Rewarded, type);
                    return new NullRewardedAdProvider();
#endif
                case AdProviderType.IronSource:
#if USE_IRONSOURCE
                    return new IronSourceRewardedProvider();
#else
                    WarnMissingDefine(AdType.Rewarded, type);
                    return new NullRewardedAdProvider();
#endif
                default:
                    return new NullRewardedAdProvider();
            }
        }

        private void WarnMissingDefine(AdType adType, AdProviderType providerType)
        {
            var symbol = AdsDefine.GetDefineSymbol(providerType);
            this.LogWarning($"{adType} provider set to {providerType} but define symbol '{symbol}' is missing. " +
                            $"Add it in Project Settings > Player > Scripting Define Symbols. Falling back to Null.");
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Banner
        // ══════════════════════════════════════════════════════════════════════

        public void ShowBanner()
        {
            if (_isSkipAds) return;
            _bannerProvider.ShowBanner();
        }

        public void HideBanner()
        {
            _bannerProvider.HideBanner();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  App Open
        // ══════════════════════════════════════════════════════════════════════

        public void ShowAppOpen(Action onComplete = null)
        {
            if (_isSkipAds)
            {
                onComplete?.Invoke();
                return;
            }

            if (!_appOpenProvider.IsAdReady())
            {
                onComplete?.Invoke();
                return;
            }

            _appOpenProvider.ShowAd(onComplete);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Interstitial
        // ══════════════════════════════════════════════════════════════════════

        public bool IsInterstitialReady()
        {
            return _interProvider.IsAdReady();
        }

        public void ShowInterstitial(
            string placement = AdsPlacement.DEFAULT,
            Action onSuccess = null,
            Action onFailure = null)
        {
            if (_isSkipAds)
            {
                onSuccess?.Invoke();
                return;
            }

            if (!CanShowInterstitial())
            {
                onFailure?.Invoke();
                return;
            }

            if (!_interProvider.IsAdReady())
            {
                onFailure?.Invoke();
                _interProvider.LoadAd();
                return;
            }

            this.Log($"ShowInterstitial — placement: {placement.Bold()}");

            _interProvider.ShowAd(
                onSuccess: () =>
                {
                    _lastInterTime = Time.realtimeSinceStartup;
                    onSuccess?.Invoke();
                },
                onFailure: () =>
                {
                    onFailure?.Invoke();
                });
        }

        private bool CanShowInterstitial()
        {
            float now = Time.realtimeSinceStartup;
            float elapsedSinceInter = now - _lastInterTime;
            float elapsedSinceReward = now - _lastRewardTime;

            return elapsedSinceInter >= _config.InterCooldown
                   && elapsedSinceReward >= _config.RewardToInterCooldown;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Rewarded
        // ══════════════════════════════════════════════════════════════════════

        public bool IsRewardedReady()
        {
            return _rewardedProvider.IsAdReady();
        }

        public void ShowRewarded(
            string placement,
            Action onSuccess,
            Action onFailure = null)
        {
            if (_isSkipAds)
            {
                onSuccess?.Invoke();
                return;
            }

            if (!_rewardedProvider.IsAdReady())
            {
                onFailure?.Invoke();
                _rewardedProvider.LoadAd();
                return;
            }

            this.Log($"ShowRewarded — placement: {placement.Bold()}");

            _rewardedProvider.ShowAd(
                onSuccess: () =>
                {
                    _lastRewardTime = Time.realtimeSinceStartup;
                    onSuccess?.Invoke();
                },
                onFailure: () =>
                {
                    onFailure?.Invoke();
                });
        }
    }
}
