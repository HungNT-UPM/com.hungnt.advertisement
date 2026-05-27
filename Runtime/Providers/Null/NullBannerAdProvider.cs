namespace HungNT.Advertisement
{
    /// <summary>
    /// Null Object implementation của <see cref="IBannerAdProvider"/>.
    /// No-op hoàn toàn — dùng khi chưa có SDK hoặc không bật define symbol.
    /// </summary>
    public class NullBannerAdProvider : IBannerAdProvider
    {
        public void Initialize(AdsConfig config)
        {
        }

        public void Dispose()
        {
        }

        public void ShowBanner()
        {
        }

        public void HideBanner()
        {
        }
    }
}