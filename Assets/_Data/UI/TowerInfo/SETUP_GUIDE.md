# Hướng dẫn Setup Tower Info UI System

## 📋 Yêu cầu hệ thống

- Unity 2021.3 LTS trở lên
- TextMeshPro package
- EventSystem (có sẵn trong Unity)

## 🚀 Bước 1: Tạo cấu trúc thư mục

```
Assets/_Data/UI/TowerInfo/
├── Scripts/
│   ├── TowerInfoData.cs
│   ├── TowerInfoDataSO.cs
│   ├── TowerInfoUI.cs
│   ├── TowerInfoTrigger.cs
│   └── TowerInfoManager.cs
├── Prefabs/
│   ├── TowerInfoUIPrefab.prefab
│   └── TowerButtonPrefab.prefab
├── Data/
│   └── (Các TowerInfoDataSO files)
└── README.md
```

## 🎯 Bước 2: Tạo TowerInfoDataSO

### 2.1 Tạo file dữ liệu
1. **Chuột phải** trong Project window
2. **Create** → **Tower Defense** → **Tower Info Data**
3. **Đặt tên** file (ví dụ: "BasicTowerInfo", "SniperTowerInfo")

### 2.2 Điền thông tin
```
Tower Name: Basic Tower
Description: A simple defensive tower with balanced stats
Base Price: 100
Tower Type: Tower
Special Abilities:
- Auto-Targeting
- Piercing Shot
- Balanced Stats
```

## 🖼️ Bước 3: Tạo UI Prefab

### 3.1 Tạo Canvas
1. **Chuột phải** trong Hierarchy
2. **UI** → **Canvas**
3. **Đặt tên**: "TowerInfoCanvas"
4. **Canvas Scaler**: Scale With Screen Size
5. **Reference Resolution**: 1920 x 1080

### 3.2 Tạo TowerInfoUI Panel
1. **Chuột phải** vào Canvas
2. **UI** → **Panel**
3. **Đặt tên**: "InfoPanel"
4. **Anchor**: Top Left
5. **Position**: (100, -100, 0)
6. **Size**: 300 x 200

### 3.3 Thêm các UI Elements
```
InfoPanel/
├── IconImage (Image)
│   ├── Size: 64x64
│   └── Position: (-120, 60, 0)
├── TowerNameText (TextMeshPro)
│   ├── Size: 200x40
│   ├── Position: (-50, 60, 0)
│   └── Font Size: 18
├── DescriptionText (TextMeshPro)
│   ├── Size: 280x60
│   ├── Position: (0, 20, 0)
│   └── Font Size: 14
├── PriceText (TextMeshPro)
│   ├── Size: 200x30
│   ├── Position: (0, -40, 0)
│   └── Font Size: 16
└── AbilitiesText (TextMeshPro)
    ├── Size: 280x80
    ├── Position: (0, -80, 0)
    └── Font Size: 12
```

### 3.4 Thêm CanvasGroup
1. **Chọn InfoPanel**
2. **Add Component** → **Canvas Group**
3. **Alpha**: 0 (để bắt đầu ẩn)

## ⚙️ Bước 4: Setup TowerInfoUI Script

### 4.1 Thêm Script
1. **Chọn Canvas**
2. **Add Component** → **TowerInfoUI**

### 4.2 Gán References
```
TowerInfoUI Component:
├── Info Panel: InfoPanel
├── Tower Icon: IconImage
├── Tower Name Text: TowerNameText
├── Description Text: DescriptionText
├── Price Text: PriceText
├── Abilities Text: AbilitiesText
├── Canvas Group: CanvasGroup
├── Fade In Time: 0.2
└── Fade Out Time: 0.1
```

## 🎮 Bước 5: Tích hợp với hệ thống chọn Tower

### 5.1 Tìm script chọn Tower hiện tại
- Tìm script quản lý việc chọn tower bằng phím số
- Thêm logic hiển thị thông tin khi chọn tower

### 5.2 Tích hợp với TowerManager (Đã hoàn thành)

Thần Thiếp đã tích hợp sẵn vào `TowerManager.cs`:

```csharp
[Header("Tower Info")]
[SerializeField] protected List<TowerInfoDataSO> towerInfoList = new List<TowerInfoDataSO>();
[SerializeField] protected bool showTowerInfo = true;
```

**Logic hoạt động:**
- Khi bấm phím số → `ShowTowerInfo()` được gọi
- Khi đặt tower → `HideTowerInfo()` được gọi
- Tự động tìm `TowerInfoDataSO` tương ứng với `TowerCode`

### 5.3 Cách hoạt động (Đã tự động)

**TowerManager đã xử lý tự động:**
- **Bấm phím số 1-5** → Tự động gọi `ShowTowerInfo()`
- **Đặt tower** → Tự động gọi `HideTowerInfo()`
- **Không cần thêm code** gì khác

## 🔧 Bước 6: Tạo TowerInfoManager (Tùy chọn)

### 6.1 Tạo GameObject
1. **Chuột phải** trong Hierarchy
2. **Create Empty**
3. **Đặt tên**: "TowerInfoManager"

### 6.2 Thêm Script
1. **Add Component** → **TowerInfoManager**
2. **Gán các TowerInfoDataSO** vào lists

## 🧪 Bước 7: Test hệ thống

### 7.1 Test cơ bản
1. **Play game**
2. **Bấm phím số** để chọn tower
3. **Kiểm tra** thông tin hiển thị ở góc trái

### 7.2 Test UI
1. **Kiểm tra** animation fade in/out
2. **Kiểm tra** vị trí hiển thị
3. **Kiểm tra** text hiển thị đúng

## 🐛 Troubleshooting

### Vấn đề thường gặp:

#### 1. UI không hiển thị
- **Kiểm tra**: Canvas có được set là Screen Space - Overlay không?
- **Kiểm tra**: EventSystem có trong scene không?
- **Kiểm tra**: References có được gán đúng không?

#### 2. Text không hiển thị
- **Kiểm tra**: TextMeshPro component có được gán đúng không?
- **Kiểm tra**: Font có được import đúng không?

#### 3. Animation không hoạt động
- **Kiểm tra**: CanvasGroup có được gán đúng không?
- **Kiểm tra**: Fade times có hợp lý không?

#### 4. Vị trí hiển thị sai
- **Kiểm tra**: Anchor và Pivot có đúng không?
- **Kiểm tra**: Screen position có đúng format không?

## 📱 Tùy chỉnh nâng cao

### Thay đổi vị trí hiển thị
```csharp
// Thay đổi vị trí trong script chọn tower
Vector3 customPosition = new Vector3(200, Screen.height - 150, 0);
TowerInfoUI.Instance.ShowTowerInfo(towerInfo, customPosition);
```

### Thay đổi animation
```csharp
// Trong TowerInfoUI Inspector
Fade In Time: 0.5  // Chậm hơn
Fade Out Time: 0.3 // Chậm hơn
```

### Thay đổi kích thước UI
1. **Chọn InfoPanel**
2. **Thay đổi Width/Height** trong RectTransform
3. **Điều chỉnh** vị trí các elements con

## ✅ Checklist hoàn thành

- [ ] Tạo cấu trúc thư mục
- [ ] Tạo ít nhất 1 TowerInfoDataSO
- [ ] Tạo UI Prefab với đầy đủ elements
- [ ] Setup TowerInfoUI script
- [ ] Tích hợp với hệ thống chọn tower
- [ ] Test hoạt động
- [ ] Tùy chỉnh vị trí và animation

## 🎯 Kết quả mong đợi

Sau khi hoàn thành setup:
- **Bấm phím số** → Chọn tower
- **Thông tin hiển thị** ở góc trái màn hình
- **Animation fade in/out** mượt mà
- **UI responsive** với mọi độ phân giải
- **Dễ dàng thay đổi** thông tin tower

## 📞 Hỗ trợ

Nếu gặp vấn đề:
1. **Kiểm tra Console** để xem error messages
2. **Kiểm tra Inspector** để xem references
3. **Kiểm tra Hierarchy** để xem cấu trúc
4. **Tham khảo README.md** để biết thêm chi tiết
