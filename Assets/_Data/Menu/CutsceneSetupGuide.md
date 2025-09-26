# HƯỚNG DẪN SETUP CUTSCENE SCENE

## **TỔNG QUAN**
Cutscene đã được tách ra thành scene riêng biệt `Hai_Cutscene` để dễ quản lý và tối ưu hiệu suất.

## **CÁC FILE ĐÃ TẠO**

### **1. CutsceneManager.cs**
- Script quản lý cutscene trong scene riêng
- Tự động tắt/bật background music
- Hỗ trợ skip cutscene
- Tự động chuyển scene sau khi kết thúc

### **2. BtnLoadCutscene.cs**
- Button để chuyển đến cutscene scene
- Có thể sử dụng trong bất kỳ scene nào

### **3. Hai_Cutscene.unity**
- Scene riêng cho cutscene (copy từ Hai_Menu.unity)

## **SETUP SCENE HAI_CUTSCENE**

### **Bước 1: Mở scene Hai_Cutscene**
1. Mở Unity Editor
2. Mở scene `Assets/_Scenes/Hai_Cutscene.unity`

### **Bước 2: Xóa các component không cần thiết**
1. **Xóa MainMenu script** khỏi GameObject chính
2. **Xóa tất cả UI elements** không liên quan đến cutscene:
   - Menu buttons (Play, New Game, Quit, etc.)
   - Confirmation dialogs
   - Menu containers

### **Bước 3: Tạo Cutscene UI**
1. **Tạo Canvas chính** (nếu chưa có):
   ```
   Canvas (Screen Space - Overlay)
   ├── CutsceneCanvas
   │   ├── VideoDisplay (RawImage)
   │   └── SkipButton (Button)
   ```

2. **Thêm CutsceneManager script** vào GameObject chính:
   - Kéo `CutsceneManager.cs` vào GameObject chính
   - Cấu hình các thông số trong Inspector

### **Bước 4: Cấu hình CutsceneManager**
1. **Cutscene Video:**
   - `Cutscene Canvas`: Kéo Canvas chứa video
   - `Video Player`: Kéo VideoPlayer component
   - `Video Display`: Kéo RawImage hiển thị video
   - `Skip Button`: Kéo Button skip
   - `Skip Button Text`: Kéo Text của skip button
   - `Cutscene Video Clip`: Kéo video clip

2. **Video Settings:**
   - `Video Width`: 1920
   - `Video Height`: 1080
   - `Maintain Aspect Ratio`: true
   - `Video Scale`: 1.0
   - `Center Video`: true

3. **Scene Settings:**
   - `Next Scene Name`: "MapSelect_Hai"
   - `Delay Before Next Scene`: 0.5
   - `Skip Button Delay`: 2.0

### **Bước 5: Tạo VideoPlayer component**
1. **Thêm VideoPlayer** vào CutsceneCanvas:
   - Right-click CutsceneCanvas → Video → Video Player
   - Cấu hình VideoPlayer:
     - `Play On Awake`: false
     - `Loop`: false
     - `Render Mode`: Render Texture

2. **Tạo RenderTexture (TÙY CHỌN - CutsceneManager sẽ tự tạo):**
   - Right-click Project → Create → Render Texture
   - Đặt tên: "CutsceneRenderTexture"
   - Cấu hình: 1920x1080
   - **LƯU Ý:** CutsceneManager sẽ tự động tạo RenderTexture, không cần tạo thủ công

3. **Gán RenderTexture cho RawImage:**
   - **LƯU Ý:** CutsceneManager sẽ tự động gán RenderTexture cho RawImage
   - Không cần gán thủ công

### **Bước 6: Tạo Skip Button**
1. **Tạo Button:**
   - Right-click CutsceneCanvas → UI → Button
   - Đặt tên: "SkipButton"
   - Đặt vị trí: góc trên bên phải

2. **Tạo Text cho Button:**
   - Right-click SkipButton → UI → Text - TextMeshPro
   - Đặt text: "SKIP"
   - Cấu hình font, size, color

3. **Ẩn Skip Button ban đầu:**
   - Uncheck "SkipButton" trong Inspector
   - CutsceneManager sẽ tự động hiện sau delay

## **CẤU HÌNH MAIN MENU**

### **Bước 1: Cập nhật MainMenu**
1. Mở scene `Hai_Menu.unity`
2. Chọn GameObject có MainMenu script
3. Trong Inspector, tìm "Scene Names":
   - `Cutscene Scene Name`: "Hai_Cutscene"

### **Bước 2: Cấu hình Play Button**
1. Chọn Play Button
2. Thay đổi OnClick event:
   - Xóa event cũ
   - Thêm event mới: MainMenu → PlayGame()

## **CÁCH SỬ DỤNG**

### **1. Từ Main Menu:**
- Bấm "PLAY" → Chuyển đến cutscene scene
- Cutscene tự động phát video
- Sau khi kết thúc → Chuyển đến Map Selection

### **2. Từ bất kỳ scene nào:**
- Sử dụng `BtnLoadCutscene` component
- Gán vào button bất kỳ
- Cấu hình `Cutscene Scene Name`

### **3. Skip Cutscene:**
- Bấm "SKIP" button (hiện sau 2 giây)
- Hoặc đợi video kết thúc tự nhiên

## **LỢI ÍCH CỦA VIỆC TÁCH CUTSCENE**

### **1. Tối ưu hiệu suất:**
- Scene cutscene nhẹ hơn
- Load nhanh hơn
- Không cần load UI không cần thiết

### **2. Dễ quản lý:**
- Cutscene logic tách biệt
- Dễ debug và sửa lỗi
- Có thể tái sử dụng

### **3. Linh hoạt:**
- Có thể load cutscene từ bất kỳ scene nào
- Dễ thay đổi video clip
- Có thể có nhiều cutscene khác nhau

## **TROUBLESHOOTING**

### **1. Video không phát:**
- Kiểm tra VideoPlayer có RenderTexture không
- Kiểm tra VideoDisplay có gán RenderTexture không
- Kiểm tra video clip có được gán không

### **2. Skip button không hiện:**
- Kiểm tra SkipButton có được gán vào CutsceneManager không
- Kiểm tra Skip Button Delay có > 0 không

### **3. Không chuyển scene sau cutscene:**
- Kiểm tra Next Scene Name có đúng không
- Kiểm tra scene có tồn tại trong Build Settings không

### **4. Background music không tắt/bật:**
- Kiểm tra SoundManager có tồn tại không
- Kiểm tra SoundManager có DontDestroyOnLoad không

## **KẾT LUẬN**

Cutscene đã được tách thành scene riêng biệt với đầy đủ tính năng:
- ✅ Tự động tắt/bật background music
- ✅ Hỗ trợ skip cutscene
- ✅ Tự động chuyển scene
- ✅ Dễ cấu hình và sử dụng
- ✅ Tối ưu hiệu suất

Bệ Hạ có thể sử dụng cutscene scene này cho bất kỳ mục đích nào! 🎬✨
