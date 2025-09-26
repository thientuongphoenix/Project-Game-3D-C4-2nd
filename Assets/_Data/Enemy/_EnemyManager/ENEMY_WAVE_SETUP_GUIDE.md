# 🎯 Enemy Wave System Setup Guide

## 📋 Tổng quan hệ thống

Hệ thống Enemy Wave cho phép spawn enemies theo từng đợt với button điều khiển:
- **5 đợt enemies** tương ứng với 5 loại enemies khác nhau
- **Button điều khiển** để bắt đầu/dừng/reset waves
- **Wood Baluster control** - tắt khi bắt đầu, bật khi hoàn thành
- **Tự động spawn** theo thứ tự từ đợt 1 đến đợt 5

## 🚀 Bước 1: Setup cơ bản

### 1.1 Tạo EnemyWaveManager
1. **Chuột phải** trong Hierarchy
2. **Create Empty** → Đặt tên "EnemyWaveManager"
3. **Add Component** → **EnemyWaveManager**

### 1.2 Tạo EnemyWaveSetup (Auto Setup)
1. **Chuột phải** vào EnemyWaveManager
2. **Create Empty** → Đặt tên "EnemyWaveSetup"
3. **Add Component** → **EnemyWaveSetup**

### 1.3 Gán Enemy Prefabs
Trong **EnemyWaveSetup** component:
```
Enemy Type 1: [Gán enemy prefab đợt 1]
Enemy Type 2: [Gán enemy prefab đợt 2]  
Enemy Type 3: [Gán enemy prefab đợt 3]
Enemy Type 4: [Gán enemy prefab đợt 4]
Enemy Type 5: [Gán enemy prefab đợt 5] (Boss)
```

## 🎮 Bước 2: Tạo UI Button

### 2.1 Tạo Canvas cho Button
1. **Chuột phải** trong Hierarchy
2. **UI** → **Canvas** → Đặt tên "EnemyWaveCanvas"
3. **Canvas Scaler**: Scale With Screen Size
4. **Reference Resolution**: 1920 x 1080

### 2.2 Tạo Button
1. **Chuột phải** vào Canvas
2. **UI** → **Button - TextMeshPro** → Đặt tên "EnemySpawnButton"
3. **Position**: Top Right (100, -100, 0)
4. **Size**: 200 x 50

### 2.3 Setup Button Text
1. **Chọn** EnemySpawnButton
2. **Text (TMP)**: "Start Enemy Waves"
3. **Font Size**: 16
4. **Color**: White

### 2.4 Tạo Status Text
1. **Chuột phải** vào Canvas
2. **UI** → **Text - TextMeshPro** → Đặt tên "StatusText"
3. **Position**: Below button (100, -160, 0)
4. **Size**: 200 x 30
5. **Text**: "Ready to start waves"
6. **Font Size**: 14

### 2.5 Add EnemySpawnButton Script
1. **Chọn** EnemySpawnButton
2. **Add Component** → **EnemySpawnButtonPrefab**
3. Script sẽ tự động setup references

## ⚙️ Bước 3: Setup Wood Baluster

### 3.1 Tìm Wood Baluster trong Scene
1. **Tìm** GameObject có tên "Wood_Baluster_01_lod0"
2. **Đảm bảo** có MeshRenderer component
3. **Ghi nhớ** tên chính xác (có thể khác tùy scene)

### 3.2 Cập nhật tên trong EnemyWaveManager
1. **Chọn** EnemyWaveManager
2. **Wood Baluster Name**: "Wood_Baluster_01_lod0" (hoặc tên chính xác)
3. **Đảm bảo** tên khớp với GameObject trong scene

## 🎯 Bước 4: Cấu hình Waves

### 4.1 Auto Setup (Khuyến nghị)
1. **Chọn** EnemyWaveSetup
2. **Auto Setup Waves**: ✅ (checked)
3. **Time Spawn**: [Gán TimeSpawn object nếu có]
4. **Enemy Prefabs**: Gán 5 enemy prefabs (Type 1-5)
5. **EnemySpawning References**: Gán 5 EnemySpawning components
6. **Wave Delay**: 2 (giây chờ giữa các đợt)

### 4.2 Manual Setup (Nếu cần)
1. **Chọn** EnemyWaveManager
2. **Waves**: Expand để thêm 5 waves
3. **Mỗi wave**:
   - **Wave Number**: 1, 2, 3, 4, 5
   - **Wave Name**: "Wave 1", "Wave 2", etc.
   - **Enemy Prefab**: [Gán enemy prefab tương ứng]
   - **Enemy Spawning**: [Gán EnemySpawning component]
   - **Wave Delay**: 2

## 🧪 Bước 5: Test hệ thống

### 5.1 Test cơ bản
1. **Play** game
2. **Click** "Start Enemy Waves" button
3. **Kiểm tra**:
   - Wood Baluster biến mất
   - Enemies spawn theo đợt
   - Status text cập nhật
   - Wood Baluster xuất hiện khi hoàn thành

### 5.2 Test Reset
1. **Sau khi** hoàn thành tất cả waves
2. **Click** "Reset Waves" button
3. **Kiểm tra**:
   - Wood Baluster xuất hiện lại
   - Button trở về "Start Enemy Waves"
   - Có thể bắt đầu lại

## 🐛 Troubleshooting

### Vấn đề thường gặp:

#### 1. Wood Baluster không tắt/bật
- **Kiểm tra**: Tên "Wood_Baluster_01_lod0" có đúng không
- **Kiểm tra**: GameObject có MeshRenderer component không
- **Kiểm tra**: Console có lỗi gì không

#### 2. Enemies không spawn
- **Kiểm tra**: EnemySpawner có trong scene không
- **Kiểm tra**: Enemy prefabs có được gán đúng không
- **Kiểm tra**: EnemyWaveSetup có hoạt động không

#### 3. Button không hoạt động
- **Kiểm tra**: EnemySpawnButton component có được add không
- **Kiểm tra**: OnClick event có được gán không
- **Kiểm tra**: EnemyWaveManager có trong scene không

#### 4. Waves không hoàn thành
- **Kiểm tra**: Enemy death detection có hoạt động không
- **Kiểm tra**: Console có lỗi gì không
- **Kiểm tra**: Enemy count có đúng không

## 📱 Tùy chỉnh nâng cao

### Thay đổi số enemies mỗi đợt
```csharp
// Trong EnemyWaveSetup
enemiesPerWave = 10; // Thay đổi từ 5 thành 10
```

### Thay đổi tốc độ spawn
```csharp
// Trong EnemyWaveSetup
spawnInterval = 0.5f; // Spawn nhanh hơn
waveDelay = 1f; // Chờ ít hơn giữa các đợt
```

### Thêm loại enemy mới
1. **Tạo** EnemyWaveData mới
2. **Gán** enemy prefab
3. **Thêm** vào waves list trong EnemyWaveManager

## ✅ Checklist hoàn thành

- [ ] EnemyWaveManager được tạo và setup
- [ ] EnemyWaveSetup được tạo và gán enemy prefabs
- [ ] UI Button được tạo và hoạt động
- [ ] Wood Baluster được tìm thấy và control được
- [ ] 5 waves được cấu hình đúng
- [ ] Test spawn enemies thành công
- [ ] Test hoàn thành waves thành công
- [ ] Test reset waves thành công

## 🎮 Cách sử dụng

1. **Bắt đầu game** → Click "Start Enemy Waves" để spawn đợt 1
2. **Tiêu diệt hết enemies đợt 1** → Button sẽ hiện "Start Next Wave"
3. **Click "Start Next Wave"** → Spawn đợt 2
4. **Lặp lại** cho đến đợt 5
5. **Hoàn thành tất cả** → Wood Baluster xuất hiện
6. **Reset** để chơi lại nếu muốn

**Lưu ý:** Người chơi phải bấm button để spawn từng đợt thay vì tự động!
