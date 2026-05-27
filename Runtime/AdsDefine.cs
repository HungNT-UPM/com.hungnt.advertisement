namespace HungNT.Advertisement
{
    /// <summary>
    /// Các hằng số liên quan đến Ads.
    /// Partial class — mở rộng thêm ở file riêng tuỳ game.
    /// </summary>
    public static partial class AdsDefine
    {
        public const string ADS_CONFIG_PATH = "Configs/AdsConfig";

        // ── Scripting Define Symbols ──────────────────────────────────────────

        public const string DEFINE_ADMOB = "USE_ADMOB";
        public const string DEFINE_MAX = "USE_MAX";
        public const string DEFINE_IRONSOURCE = "USE_IRONSOURCE";

        /// <summary>
        /// Trả về define symbol tương ứng với <see cref="AdProviderType"/>.
        /// </summary>
        public static string GetDefineSymbol(AdProviderType providerType)
        {
            switch (providerType)
            {
                case AdProviderType.AdMob:       return DEFINE_ADMOB;
                case AdProviderType.Max:         return DEFINE_MAX;
                case AdProviderType.IronSource:  return DEFINE_IRONSOURCE;
                default:                         return "";
            }
        }
    }
}
