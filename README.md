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

## Phụ thuộc

`com.hungnt.core`, SDK quảng cáo (AdMob / MAX / IronSource) và define symbol trong project consumer.

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

---

## 3. AdsConfig

1. **Create > HungNT > Ads > Ads Config**
2. Đặt tại `Assets/.../Resources/Configs/AdsConfig.asset` (tên file **`AdsConfig`**)
3. **HungNT > Ads > Open Ads Config**
4. Tab **Providers** + tab keys từng SDK

---

## 4. Sử dụng trong game

```csharp
var ads = this.GetService<IAdsService>();

ads.ShowBanner();
ads.ShowInterstitial(AdsPlacement.LEVEL_COMPLETE, onSuccess: () => { }, onFailure: () => { });
ads.ShowRewarded(AdsPlacement.DOUBLE_COIN, onSuccess: GiveReward, onFailure: () => { });
ads.IsSkipAds = true;
```

Demo: gắn **`AdsServiceDemo`**, Play Mode → nút Inspector.

---

## 5. Mở rộng

Partial class **`AdsPlacement`**, thêm **`AdProvider`** + factory trong `AdsService` nếu tích hợp SDK mới.

---

## 6. Test trong Unity Editor

AdMob placeholder cần **EventSystem** — `AdsService.Initialize` tự tạo trong Editor nếu thiếu.
