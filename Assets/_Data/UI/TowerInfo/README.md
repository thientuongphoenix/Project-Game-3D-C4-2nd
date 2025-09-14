# Tower Info UI System - Hiển thị thông tin Tower

Hệ thống UI hiển thị thông tin Towers và Traps trong game Tower Defense, tích hợp với hệ thống chọn tower có sẵn và hiển thị ở **góc trái màn hình**.

> 📖 **Xem hướng dẫn setup chi tiết**: [SETUP_GUIDE.md](SETUP_GUIDE.md)

## Tổng quan

Hệ thống này cung cấp:
- **TowerInfoData**: Lưu trữ thông tin cơ bản của Tower/Trap
- **TowerInfoUI**: Hiển thị thông tin tower ở góc trái màn hình
- **TowerInfoTrigger**: Kích hoạt hiển thị thông tin khi click (tùy chọn)
- **TowerInfoManager**: Quản lý database thông tin của tất cả Towers/Traps

## Thông tin hiển thị

### Thông tin cơ bản
- **Tên Tower/Trap**
- **Mô tả chức năng**
- **Icon** (nếu có)

### Thông tin giá
- **Giá mua ban đầu** (basePrice)

### Khả năng đặc biệt
- **Special Abilities**: Các khả năng đặc biệt của Tower/Trap

## Cách sử dụng

### 1. Thiết lập cơ bản

#### Cách 1: Sử dụng ScriptableObject (Khuyến nghị)
1. **Tạo TowerInfoDataSO**:
   - Chuột phải trong Project → Create → Tower Defense → Tower Info Data
   - Đặt tên file (ví dụ: "BasicTowerInfo")
   - Điền thông tin trong Inspector

2. **Gán vào TowerInfoTrigger**:
   - Kéo TowerInfoDataSO vào field `Tower Info SO` của TowerInfoTrigger

#### Cách 2: Sử dụng TowerInfoData trực tiếp
1. **Tạo TowerInfoData** trong Inspector:
   - Thêm TowerInfoTrigger component vào Tower/Trap
   - Điền thông tin trực tiếp vào field `Tower Info`

#### Cách 3: Tự động tạo
1. **Bật Auto Create**:
   - Đặt `Auto Create Info = true` trong TowerInfoTrigger
   - Hệ thống sẽ tự động tạo thông tin từ TowerCtrl

### 2. Sử dụng trong Tower/Trap (Tùy chọn)

1. **Thêm TowerInfoTrigger component** vào Tower/Trap prefab
2. **Gán TowerInfoData** vào component
3. **Hoặc để tự động tạo** từ TowerCtrl

### 3. Hiển thị thông tin Tower

1. **Sử dụng chức năng chọn tower có sẵn** của Bệ Hạ
2. **Thông tin tự động hiển thị** ở góc trái màn hình với animation fade in/out
3. **Hoặc click vào Tower/Trap** để xem thông tin (nếu có TowerInfoTrigger)

## Cấu trúc dữ liệu

### TowerInfoData (Class)
- Lưu trữ thông tin cơ bản của Tower/Trap
- Có thể tạo trực tiếp trong Inspector

### TowerInfoDataSO (ScriptableObject)
- Lưu trữ thông tin dưới dạng asset file
- Dễ dàng tái sử dụng và quản lý
- Có thể tạo từ menu Create

### TowerInfoUI (Thông tin chi tiết)
- Icon của Tower/Trap
- Tên và mô tả
- Giá tiền
- Khả năng đặc biệt

## Tính năng

### Tự động tạo thông tin
- Từ TowerCtrl component
- Từ các thông số có sẵn (price)
- Tạo mô tả mặc định dựa trên loại

### Hỗ trợ đa ngôn ngữ
- Tất cả text đều có thể thay đổi
- Hỗ trợ Unicode (tiếng Việt)

### Animation và Visual Feedback
- Fade in/out cho thông tin tower sử dụng Coroutine
- Vị trí hiển thị cố định ở góc trái màn hình

### Tích hợp với hệ thống hiện tại
- Sử dụng SaiSingleton pattern
- Tương thích với TowerCtrl, TowerType
- Hỗ trợ PoolObj system

## Tùy chỉnh

### Thêm Tower/Trap mới
1. Tạo TowerInfoData mới
2. Thêm vào TowerInfoManager
3. Tạo prefab với TowerInfoTrigger

### Thay đổi UI
1. Chỉnh sửa prefab UI
2. Cập nhật script tương ứng
3. Gán reference mới

### Thay đổi logic
1. Override các method virtual
2. Kế thừa từ class base
3. Sử dụng events và callbacks

## Lưu ý

- Cần TextMeshPro để hiển thị text
- Sử dụng Coroutine tích hợp sẵn của Unity cho animation
- UI cần Canvas và EventSystem để hoạt động
- Tower/Trap cần Collider để trigger events

## Ví dụ sử dụng

```csharp
// Hiển thị thông tin tower ở góc trái màn hình
TowerInfoData info = TowerInfoManager.Instance.GetTowerInfo("Basic Tower");
Vector3 leftCornerPosition = new Vector3(100, Screen.height - 100, 0);
TowerInfoUI.Instance.ShowTowerInfo(info, leftCornerPosition);

// Tự động hiển thị thông tin khi chọn tower
// (tích hợp với hệ thống chọn tower có sẵn của Bệ Hạ)
```

## Ưu điểm của hệ thống hiển thị thông tin

1. **Tích hợp dễ dàng**: Sử dụng hệ thống chọn tower có sẵn
2. **Hiệu quả**: Thông tin hiển thị ở vị trí cố định, dễ đọc
3. **Không xâm lấn**: Chỉ hiển thị khi cần, không chiếm màn hình
4. **Linh hoạt**: Có thể tùy chỉnh vị trí hiển thị
5. **Chuyên nghiệp**: Giao diện đẹp với animation fade in/out
6. **Dễ bảo trì**: Không cần thay đổi logic chọn tower hiện tại
