# HungNT Advertisement (`com.hungnt.advertisement`)

Lớp trừu tượng quảng cáo **`IAdsService`**, nhiều SDK (AdMob, AppLovin MAX, IronSource) theo **Strategy Pattern** — mỗi format (Banner / Interstitial / Rewarded / App Open) chọn provider riêng trong **`AdsConfig`**.

## Tính năng

- **`IAdsService`** — Banner, Interstitial, Rewarded, App Open, cooldown, skip ads (VIP)
- **Provider tách riêng** — mix AdMob + MAX + IronSource trên từng format
- **`AdsConfig`** ScriptableObject — tab Providers + tab keys từng SDK
- **Null providers** — fallback an toàn khi thiếu SDK / define symbol
- Tích hợp **`ServiceLocator`** / `GetService<IAdsService>()`
- Demo assembly **`HungNT.Advertisement.Demo`** — `AdsServiceDemo.cs`

| SDK | Formats | Define |
|-----|---------|--------|
| Google AdMob | App Open, Banner, Inter, Rewarded | `USE_ADMOB` |
| AppLovin MAX | Banner, Inter, Rewarded | `USE_MAX` |
| IronSource LevelPlay | Banner, Inter, Rewarded | `USE_IRONSOURCE` |

> App Open mặc định chỉ AdMob; chọn MAX/IronSource cho App Open → Null provider.

## Cài đặt

### OpenUPM

Cần scope AppLovin nếu dùng MAX (`com.applovin.mediation.ads`):

```json
"scopedRegistries": [
  {
    "name": "OpenUPM",
    "url": "https://package.openupm.com",
    "scopes": ["com.hungnt", "com.cysharp"]
  },
  {
    "name": "AppLovin MAX Unity",
    "url": "https://unity.packages.applovin.com/",
    "scopes": ["com.applovin.mediation.ads"]
  }
],
"dependencies": {
  "com.hungnt.advertisement": "1.0.3"
}
```

### GitHub

```json
"com.hungnt.advertisement": "https://github.com/HungNT-UPM/com.hungnt.advertisement.git#v1.0.3"
```

**Phụ thuộc:** `com.hungnt.core`. SDK quảng cáo import và define symbol trong **project consumer** (xem bên dưới).

---

## 1. Download & Import SDK

### Google AdMob

1. https://github.com/googleads/googleads-mobile-unity/releases → import `.unitypackage`
2. **Assets > External Dependency Manager > Android Resolver > Force Resolve**
3. **Assets > Google Mobile Ads > Settings** → App ID Android/iOS

### AppLovin MAX

1. https://github.com/AppLovin/AppLovin-MAX-Unity-Plugin/releases
2. **AppLovin > Integration Manager** → adapter networks
3. Force Resolve dependencies

### IronSource (LevelPlay)

1. https://developers.is.com/ironsource-mobile/unity/levelplay-starter-kit/
2. **LevelPlay > Mediation > SDK Integration**
3. Force Resolve dependencies

---

## 2. Define symbols

**Edit > Project Settings > Player > Scripting Define Symbols**

- `USE_ADMOB` — đã import Google Mobile Ads  
- `USE_MAX` — đã import AppLovin MAX  
- `USE_IRONSOURCE` — đã import IronSource  

Ví dụ: `USE_ADMOB;USE_MAX`

> Chưa import SDK mà thêm symbol → lỗi compile. Có SDK mà thiếu symbol → provider không được build (an toàn).

---

## 3. AdsConfig

1. **Create > HungNT > Ads > Ads Config**
2. Đặt tại `Assets/.../Resources/Configs/AdsConfig.asset` (tên file **`AdsConfig`**)
3. **HungNT > Ads > Open Ads Config**
4. Tab **Providers**: chọn SDK từng format; **None** = tắt format đó
5. Tab keys: điền Ad Unit / SDK Key / App Key

Ví dụ mix-provider:

```
App Open      = AdMob
Banner        = MAX
Interstitial  = IronSource
Rewarded      = MAX
```

---

## 4. Sử dụng trong game

### Register

Gắn `AdsService` lên GameObject có `ServiceRegister`, hoặc:

```csharp
ServiceLocator.Instance.Register<IAdsService>(adsServiceInstance);
```

### Gọi ads

```csharp
var ads = this.GetService<IAdsService>();

ads.ShowBanner();
ads.HideBanner();

ads.ShowAppOpen(onComplete: () => Debug.Log("App open done"));

ads.ShowInterstitial(
    placement: AdsPlacement.LEVEL_COMPLETE,
    onSuccess: () => Debug.Log("Inter shown"),
    onFailure: () => Debug.Log("Inter failed/cooldown")
);

ads.ShowRewarded(
    placement: AdsPlacement.DOUBLE_COIN,
    onSuccess: () => GiveReward(),
    onFailure: () => Debug.Log("Reward failed")
);

ads.IsSkipAds = true; // VIP / no-ads
```

### Demo

Gắn **`AdsServiceDemo`** (`Demo/AdsServiceDemo.cs`), Play Mode → nút Inspector: Banner, Interstitial, Rewarded, App Open, check ready state.

---

## 5. Mở rộng

**Placement mới** — partial class:

```csharp
public static partial class AdsPlacement
{
    public const string MY_CUSTOM = "my_custom";
}
```

**SDK mới:** thêm `AdProvider`, implement `IBannerAdProvider` / …, factory trong `AdsService`, `#if USE_XXX`, cập nhật `InitializeSdks()`.

---

## 6. Test trong Unity Editor

- AdMob dùng **placeholder UI** (`Assets/GoogleMobileAds/Editor/Resources/PlaceholderAds/`).
- Placeholder cần **EventSystem** — `AdsService.Initialize` tự tạo trong Editor nếu thiếu.
- Nút Close không bấm được: kiểm tra Canvas khác có **Sort Order** cao che placeholder.
