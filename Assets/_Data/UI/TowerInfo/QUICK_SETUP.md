# 🚀 Quick Setup - Tower Info UI System

## ⚡ Setup trong 5 phút

### 1️⃣ Tạo TowerInfoDataSO
```
Chuột phải Project → Create → Tower Defense → Tower Info Data
Đặt tên: "BasicTowerInfo"
Điền: Tên, mô tả, giá, loại, khả năng
```

### 2️⃣ Tạo UI
```
Hierarchy → UI → Canvas
Thêm Panel con → Đặt tên "InfoPanel"
Thêm: Image (icon), 4x TextMeshPro (tên, mô tả, giá, khả năng)
Thêm CanvasGroup → Alpha = 0
```

### 3️⃣ Setup Script
```
Chọn Canvas → Add Component → TowerInfoUI
Gán references: InfoPanel, các Text, Image, CanvasGroup
```

### 4️⃣ Tích hợp với TowerManager (Đã hoàn thành)

**Thần Thiếp đã tích hợp sẵn vào TowerManager:**
- Thêm `towerInfoList` để chứa các TowerInfoDataSO
- Tự động hiển thị thông tin khi bấm phím số
- Tự động ẩn thông tin khi đặt tower
- **Không cần thêm code** gì khác!

### 5️⃣ Test
```
Play → Bấm phím số → Xem thông tin hiển thị
```

## 🎯 Kết quả
- ✅ Thông tin tower hiển thị ở góc trái
- ✅ Animation fade in/out mượt mà
- ✅ Tích hợp với hệ thống chọn tower có sẵn

## 📖 Chi tiết đầy đủ
Xem [SETUP_GUIDE.md](SETUP_GUIDE.md) để biết thêm chi tiết và troubleshooting.
