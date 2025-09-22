# Hướng dẫn Setup UI Hướng dẫn Sử dụng Vật phẩm

## Tổng quan
Hệ thống UI hướng dẫn sử dụng vật phẩm sẽ hiển thị khi enemy chết và rớt vật phẩm, hướng dẫn người chơi bấm phím I để mở inventory và sử dụng vật phẩm.

## Các file đã tạo:
1. `ItemGuideUI.cs` - Quản lý UI hướng dẫn
2. `ItemUseTracker.cs` - Theo dõi việc sử dụng vật phẩm
3. Cập nhật `BtnUseItem.cs` - Thông báo khi sử dụng vật phẩm
4. Cập nhật `EnemyDamageReceiver.cs` - Hiển thị UI khi enemy chết

## Setup UI trong Unity:

### 1. Tạo GameObject cho ItemGuideUI:
```
Canvas (hoặc UI Canvas hiện có)
└── ItemGuideUI (GameObject)
    └── GuidePanel (GameObject)
        ├── GuideText (Text Component)
        └── CloseButton (Button Component)
```

### 2. Cấu hình các component:

#### ItemGuideUI GameObject:
- Thêm script `ItemGuideUI`
- Assign các reference:
  - `GuidePanel`: GameObject chứa UI
  - `GuideText`: Text component hiển thị hướng dẫn
  - `CloseButton`: Button để đóng UI

#### GuidePanel:
- Có thể là Panel hoặc Image với background
- Đặt vị trí phù hợp trên màn hình (ví dụ: giữa màn hình)

#### GuideText:
- TextMeshPro - Text (UI) component hiển thị: "Bạn đã nhận được vật phẩm! Nhấn phím I để mở inventory và sử dụng vật phẩm."
- Có thể tùy chỉnh font, size, color, style với TextMeshPro

#### CloseButton:
- Button component để người chơi có thể đóng UI thủ công
- Có thể ẩn nếu muốn UI chỉ đóng tự động

### 3. Cấu hình trong Inspector:

#### ItemGuideUI:
- `Guide Panel`: Drag GuidePanel GameObject
- `Guide Text`: Drag GuideText TextMeshPro - Text (UI) component
- `Close Button`: Drag CloseButton Button component
- `Auto Hide Delay`: 10 (auto hide after 10 seconds)
- `Guide Message`: "You received items! Press I key to open inventory and use items."

## Cách hoạt động:

### Khi enemy chết:
1. `EnemyDamageReceiver.RewardOnDead()` được gọi
2. Vật phẩm được rớt ra (Gold, Exp, HealthPotion, ManaPotion)
3. `ShowItemGuide()` được gọi
4. **UI hướng dẫn chỉ hiển thị lần đầu tiên** với thông báo: *"You received items! Press I key to open inventory and use items."*

### UI tự động ẩn khi:
1. **Người chơi bấm phím I** (mở inventory)
2. **Người chơi bấm button USE** (sử dụng vật phẩm)
3. **Sau 10 giây** (auto hide)
4. **Người chơi bấm CloseButton** (nếu có)

### Tính năng "Show Once":
- **UI chỉ hiển thị một lần duy nhất** trong suốt game session
- Sau khi hiển thị lần đầu, các enemy chết tiếp theo sẽ không hiển thị UI nữa
- Sử dụng `ResetGuideState()` để reset và cho phép hiển thị lại (hữu ích khi bắt đầu game mới)

## Tùy chỉnh:

### Thay đổi thông báo:
```csharp
ItemGuideUI.Instance.SetGuideMessage("Thông báo mới của bạn");
```

### Thay đổi thời gian auto hide:
```csharp
// Trong Inspector của ItemGuideUI
autoHideDelay = 15f; // 15 giây
```

### Thay đổi vị trí hiển thị:
- Điều chỉnh RectTransform của GuidePanel
- Có thể sử dụng Canvas Scaler để responsive

### Quản lý trạng thái "Show Once":
```csharp
// Kiểm tra xem guide đã hiển thị chưa
bool hasShown = ItemGuideUI.Instance.HasShownOnce();

// Reset trạng thái để cho phép hiển thị lại
ItemGuideUI.Instance.ResetGuideState();

// Kiểm tra guide có đang hiển thị không
bool isShowing = ItemGuideUI.Instance.IsShowing();
```

## Lưu ý:
- Đảm bảo ItemGuideUI GameObject được đặt trong scene
- Có thể tạo prefab để sử dụng lại
- UI sẽ tự động hoạt động khi enemy chết và rớt vật phẩm
