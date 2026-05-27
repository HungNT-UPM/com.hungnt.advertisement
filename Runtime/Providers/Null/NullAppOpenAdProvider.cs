using System;

namespace HungNT.Advertisement
{
    /// <summary>
    /// Null Object implementation của <see cref="IAppOpenAdProvider"/>.
    /// No-op hoàn toàn — dùng khi chưa có SDK hoặc không bật define symbol.
    /// </summary>
    public class NullAppOpenAdProvider : IAppOpenAdProvider
    {
        public void Initialize(AdsConfig config)
        {
        }

        public void Dispose()
        {
        }

        public void LoadAd()
        {
        }

        public bool IsAdReady()
        {
            return false;
        }

        public void ShowAd(Action onComplete)
        {
            onComplete?.Invoke();
        }
    }
}