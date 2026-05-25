# Ads Service — Hướng dẫn cài đặt

## 1. Tổng quan

Hệ thống Ads sử dụng **Strategy Pattern** với các provider riêng cho từng SDK. Mỗi loại ads (AppOpen / Banner / Interstitial / Rewarded) có thể chọn **provider riêng** trong `AdsConfig` — ví dụ Inter dùng IronSource còn Rewarded dùng MAX.

| SDK | Ad Formats | Define Symbol |
|-----|-----------|---------------|
| **Google AdMob** | App Open, Banner, Interstitial, Rewarded | `USE_ADMOB` |
| **AppLovin MAX** | Banner, Interstitial, Rewarded | `USE_MAX` |
| **IronSource (LevelPlay)** | Banner, Interstitial, Rewarded | `USE_IRONSOURCE` |

> **Lưu ý:** App Open chỉ có sẵn cho AdMob trong bộ provider mặc định. Chọn MAX/IronSource cho App Open sẽ fallback về Null.

---

## 2. Download & Import SDK

### 2.1 Google AdMob (Google Mobile Ads Unity Plugin)

1. Tải plugin tại: https://github.com/googleads/googleads-mobile-unity/releases
2. Import file `.unitypackage` vào project
3. Sau khi import, vào menu **Assets > External Dependency Manager > Android Resolver > Force Resolve**
4. Cấu hình App ID:
   - Vào **Assets > Google Mobile Ads > Settings**
   - Điền **AdMob App ID** cho Android và iOS (lấy từ AdMob Console)

### 2.2 AppLovin MAX

1. Tải plugin tại: https://github.com/AppLovin/AppLovin-MAX-Unity-Plugin/releases
2. Import file `.unitypackage` vào project
3. Sau khi import, vào **AppLovin > Integration Manager** để cài thêm các ad network adapter (Meta, AdMob adapter, Unity Ads, ...)
4. Resolve dependencies: **Assets > External Dependency Manager > Android Resolver > Force Resolve**

### 2.3 IronSource (Unity LevelPlay)

1. Tải plugin tại: https://developers.is.com/ironsource-mobile/unity/levelplay-starter-kit/
2. Import file `.unitypackage` vào project
3. Vào **LevelPlay > Mediation > SDK Integration** để cài adapter
4. Resolve dependencies: **Assets > External Dependency Manager > Android Resolver > Force Resolve**

---

## 3. Thêm Define Symbols

1. Vào **Edit > Project Settings > Player > Other Settings > Scripting Define Symbols**
2. Thêm symbol tương ứng với SDK đã import:
   - `USE_ADMOB` — khi đã import Google Mobile Ads
   - `USE_MAX` — khi đã import AppLovin MAX
   - `USE_IRONSOURCE` — khi đã import IronSource

Ví dụ: `USE_ADMOB;USE_MAX`

> **Quan trọng:** Nếu chưa import SDK mà thêm define symbol sẽ gây lỗi biên dịch. Ngược lại, nếu đã import SDK nhưng chưa thêm symbol thì provider sẽ không được biên dịch (an toàn).

---

## 4. Lấy Ad Keys

### 4.1 AdMob

1. Đăng nhập https://admob.google.com
2. Tạo App → lấy **App ID** (dạng `ca-app-pub-XXXXX~YYYYY`)
3. Tạo Ad Unit cho từng format → lấy **Ad Unit ID** (dạng `ca-app-pub-XXXXX/ZZZZZ`)
4. Các test key đã được điền sẵn trong AdsConfig (default values)

### 4.2 AppLovin MAX

1. Đăng nhập https://dash.applovin.com
2. Vào **Account > General > Keys** → lấy **SDK Key**
3. Vào **MAX > Manage > Ad Units** → tạo ad unit cho từng format + platform → lấy **Ad Unit ID**
4. MAX không có universal test ID — phải tạo ad unit riêng trên dashboard

### 4.3 IronSource

1. Đăng nhập https://platform.ironsrc.com
2. Tạo App → lấy **App Key** cho từng platform
3. IronSource sử dụng App Key chung cho tất cả ad format (không cần ad unit ID riêng)

---

## 5. Tạo & Cấu hình AdsConfig

1. Trong Unity Editor, tạo file config:
   - Right-click trong Project window → **Create > HungNT > Ads > Ads Config**
2. Đặt file tại: `Assets/.../Resources/Configs/AdsConfig.asset`
   - Tạo folder `Resources/Configs/` nếu chưa có
   - Tên file phải là `AdsConfig`
3. Mở config bằng menu: **HungNT > Ads > Open Ads Config**
4. Cấu hình:
   - **Tab "Providers"**: chọn SDK cho từng loại ads (AppOpen / Banner / Interstitial / Rewarded). Đặt `None` để vô hiệu hoá loại ads đó.
   - **Tab "AdMob" / "MAX" / "IronSource"**: điền ad keys tương ứng với SDK đã chọn.

### Ví dụ cấu hình mix-provider

```
App Open      = AdMob
Banner        = MAX
Interstitial  = IronSource
Rewarded      = MAX
```

> Cấu hình này yêu cầu cả ba define symbol `USE_ADMOB;USE_MAX;USE_IRONSOURCE` và đã import đủ 3 SDK.
> Nếu chọn provider mà thiếu define symbol, hệ thống sẽ fallback về Null và log warning trong Console.

---

## 6. Sử dụng trong Game

### Register AdsService

Gắn `AdsService` component lên GameObject có `ServiceRegister`, hoặc register thủ công:

```csharp
ServiceLocator.Instance.Register<IAdsService>(adsServiceInstance);
```

### Gọi Ads

```csharp
var ads = this.GetService<IAdsService>();

// Banner
ads.ShowBanner();
ads.HideBanner();

// App Open
ads.ShowAppOpen(onComplete: () => Debug.Log("App open done"));

// Interstitial (có cooldown tự động)
ads.ShowInterstitial(
    placement: AdsPlacement.LEVEL_COMPLETE,
    onSuccess: () => Debug.Log("Inter shown"),
    onFailure: () => Debug.Log("Inter failed/cooldown")
);

// Rewarded
ads.ShowRewarded(
    placement: AdsPlacement.DOUBLE_COIN,
    onSuccess: () => GiveReward(),
    onFailure: () => Debug.Log("Reward failed")
);

// Skip ads (VIP / no-ads purchase)
ads.IsSkipAds = true;
```

---

## 7. Mở rộng

- **Thêm placement mới:** Tạo file partial class `AdsPlacement` riêng:

```csharp
public static partial class AdsPlacement
{
    public const string MY_CUSTOM_PLACEMENT = "my_custom_placement";
}
```

- **Thêm constants:** Tạo file partial class `AdsDefine` riêng (vd: remote config keys)

- **Thêm SDK provider mới:**
  1. Thêm giá trị mới vào enum `AdProvider`
  2. Implement interface tương ứng (`IBannerAdProvider`, `IInterstitialAdProvider`, ...)
  3. Thêm `case` trong các factory `CreateXxxProvider` của `AdsService`, kèm `#if USE_XXX`
  4. Cập nhật `AdsService.InitializeSdks()` nếu SDK cần init bất đồng bộ

---

## 8. Lưu ý khi test trong Unity Editor

- AdMob hiển thị **placeholder UI** trong Editor (prefab tại `Assets/GoogleMobileAds/Editor/Resources/PlaceholderAds/`). Placeholder dùng `UnityEngine.UI.Button` để hiển thị nút Close Ad.
- Placeholder cần **EventSystem** trong scene để bấm được nút Close. `AdsService.Initialize` đã tự tạo EventSystem khi chạy trong Editor nếu scene chưa có (chỉ có hiệu lực `UNITY_EDITOR`). Trên thiết bị thật, ads là native overlay nên không cần EventSystem.
- Nếu vẫn không bấm được close: kiểm tra Canvas khác trong scene có `Sorting Order` cao hơn 0 → placeholder bị che. Hạ Sort Order của Canvas đó hoặc tạm tắt khi test ads.
