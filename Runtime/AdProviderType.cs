namespace HungNT.Advertisement
{
    /// <summary>
    /// Các SDK quảng cáo được hỗ trợ. Dùng trong <see cref="AdsConfig"/>
    /// để gán provider riêng cho từng loại ads (AppOpen / Banner / Interstitial / Rewarded).
    /// </summary>
    public enum AdProviderType
    {
        None = 0,
        AdMob = 1,
        Max = 2,
        IronSource = 3,
    }
}