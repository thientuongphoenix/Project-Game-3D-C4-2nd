using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TowerQuest
{
    public string questName;
    public string description;
    public int requiredTowerCount;
    public TowerCode unlockedTower;
    public bool isCompleted;
    public bool isUnlocked;
}

public class TowerQuestSystem : SaiSingleton<TowerQuestSystem>
{
    [Header("Quest Settings")]
    [SerializeField] protected List<TowerQuest> towerQuests = new List<TowerQuest>();
    [SerializeField] protected int totalTowersPlaced = 0;
    [SerializeField] protected int oneGunBarrelTowersPlaced = 0; // Add variable to count OneGunBarrel towers
    [SerializeField] protected int iceTrapTowersPlaced = 0; // Add variable to count Ice Trap towers
    [SerializeField] protected int movementTutorialCompleted = 0; // Add variable to count movement tutorial
    [SerializeField] protected bool finalMissionCompleted = false; // Add variable for final mission completion
    
    [Header("UI")]
    [SerializeField] protected GameObject questNotificationPrefab;
    
    [Header("Scene Management")]
    [SerializeField] protected bool isDestroyed = false; // Track if this instance has been destroyed
    
    protected override void LoadComponents()
    {
        try
        {
            base.LoadComponents();
            
            Debug.Log("TowerQuestSystem: Starting LoadComponents");
            
            // Kiểm tra scene hiện tại - chỉ hoạt động ở Hai_SampleScene
            if (!this.IsValidScene())
            {
                Debug.Log("TowerQuestSystem: Not in Hai_SampleScene, destroying instance");
                this.DestroyInstance();
                return;
            }
            
            // DON'T delete old quests anymore - only create new quests if needed
            // this.CleanupOldQuests();
            
            this.InitializeQuests();
            
            Debug.Log("TowerQuestSystem: LoadComponents completed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Serious error in LoadComponents: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            
            // Reinitialize from scratch if there's an error
            try
            {
                towerQuests = new List<TowerQuest>();
                totalTowersPlaced = 0;
                this.InitializeQuests();
                Debug.Log("Recovered TowerQuestSystem after error");
            }
            catch (System.Exception recoveryError)
            {
                Debug.LogError($"Cannot recover TowerQuestSystem: {recoveryError.Message}");
            }
        }
    }
    
    protected virtual void CleanupOldQuests()
    {
        try
        {
            // Kiểm tra an toàn trước khi xóa
            if (towerQuests == null)
            {
                Debug.LogWarning("towerQuests list bị null, khởi tạo lại");
                towerQuests = new List<TowerQuest>();
                return;
            }
            
            // Xóa tất cả quest cũ không mong muốn
            int removedCount = towerQuests.RemoveAll(q => q != null && q.unlockedTower != TowerCode.OneGunBarrel);
            if (removedCount > 0)
            {
                Debug.Log($"Đã xóa {removedCount} quest cũ không mong muốn");
            }
            
            // Reset counter
            totalTowersPlaced = 0;
            
            Debug.Log("CleanupOldQuests hoàn thành thành công");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong CleanupOldQuests: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            
            // Khởi tạo lại list nếu có lỗi
            towerQuests = new List<TowerQuest>();
            totalTowersPlaced = 0;
        }
    }
    
    protected virtual void InitializeQuests()
    {
        try
        {
            // Kiểm tra an toàn trước khi thêm
            if (towerQuests == null)
            {
                Debug.LogWarning("towerQuests list bị null, khởi tạo lại");
                towerQuests = new List<TowerQuest>();
            }
            
            // Kiểm tra xem có phải Map 1 không - nếu là Map 1 thì không tạo quest
            if (this.IsMap1())
            {
                Debug.Log("Map 1 detected - Skipping quest initialization completely!");
                // Xóa tất cả quest cũ nếu có
                towerQuests.Clear();
                Debug.Log("Map 1: Cleared all quests - No quests will be created!");
                return;
            }
            
            // XÓA QUEST CŨ VỀ CORE TOWER TRƯỚC KHI TẠO QUEST MỚI
            towerQuests.RemoveAll(q => q != null && q.unlockedTower == TowerCode.Core);
            Debug.Log("Đã xóa quest cũ về Core Tower");
            
            // Kiểm tra xem đã có quest tutorial di chuyển chưa
            bool hasMovementTutorial = towerQuests.Find(q => q.questName == "Movement Tutorial") != null;
            
            if (!hasMovementTutorial)
            {
                // Thêm quest tutorial di chuyển đầu tiên
                towerQuests.Add(new TowerQuest
                {
                    questName = "Movement Tutorial",
                    description = "Move using WASD keys to move, Left Shift to sprint and Space to jump to get familiar with controls",
                    requiredTowerCount = 1,
                    unlockedTower = TowerCode.MachineGun, // Unlock MachineGun tower
                    isCompleted = false,
                    isUnlocked = false
                });
                
                Debug.Log("Đã thêm quest tutorial: Movement Tutorial");
            }
            
            // Kiểm tra xem đã có quest "Tower Builder I" chưa
            bool hasFirstQuest = towerQuests.Find(q => q.questName == "Tower Builder I") != null;
            
            if (!hasFirstQuest)
            {
                // Chỉ thêm quest mới nếu chưa có
                towerQuests.Add(new TowerQuest
                {
                    questName = "Tower Builder I",
                    description = "Place 3 towers to unlock OneGunBarrel Tower",
                    requiredTowerCount = 3,
                    unlockedTower = TowerCode.OneGunBarrel,
                    isCompleted = false,
                    isUnlocked = false  // IMPORTANT: Quest starts with isUnlocked = false
                });
                
                Debug.Log("Đã thêm quest mới: Tower Builder I");
            }
            else
            {
                Debug.Log("Quest 'Tower Builder I' đã tồn tại, không thêm mới");
            }
            
            // KHÔNG tạo quest thứ hai ở đây nữa - sẽ được tạo tự động khi hoàn thành quest đầu tiên
            // KHÔNG tạo final mission ở đây nữa - sẽ được tạo khi hoàn thành Tower Builder III
            
            // Debug: Kiểm tra tất cả quest hiện có
            Debug.Log($"Tổng số quest hiện tại: {towerQuests.Count}");
            foreach (var quest in towerQuests)
            {
                Debug.Log($"DEBUG: Quest '{quest.questName}' - UnlockedTower: {quest.unlockedTower}, RequiredCount: {quest.requiredTowerCount}, Completed: {quest.isCompleted}");
            }
            
            // Debug đặc biệt cho Movement Tutorial
            var movementQuest = towerQuests.Find(q => q.questName == "Movement Tutorial");
            if (movementQuest != null)
            {
                Debug.Log($"MOVEMENT TUTORIAL DEBUG: Found quest '{movementQuest.questName}' - Required: {movementQuest.requiredTowerCount}, Completed: {movementQuest.isCompleted}");
                Debug.Log($"MOVEMENT TUTORIAL DEBUG: movementTutorialCompleted = {movementTutorialCompleted}");
            }
            else
            {
                Debug.LogWarning("KHÔNG TÌM THẤY Movement Tutorial quest!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong InitializeQuests: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            
            // Khởi tạo lại list nếu có lỗi
            towerQuests = new List<TowerQuest>();
        }
    }
    
    
    public virtual void OnTowerPlaced(TowerCode towerType = TowerCode.NoTower)
    {
        // Kiểm tra nếu instance đã bị destroy hoặc không ở scene hợp lệ
        if (this.isDestroyed || !this.IsValidScene())
        {
            Debug.LogWarning("TowerQuestSystem: Instance destroyed or not in valid scene, ignoring OnTowerPlaced");
            return;
        }
        
        Debug.Log($"=== ON TOWER PLACED CALLED ===");
        Debug.Log($"TowerType được truyền vào: {towerType}");
        
        // Đếm riêng OneGunBarrel towers - KHÔNG đếm vào totalTowersPlaced
        if (towerType == TowerCode.OneGunBarrel)
        {
            oneGunBarrelTowersPlaced++;
            Debug.Log($"OneGunBarrel Tower đã được đặt! Tổng số OneGunBarrel: {oneGunBarrelTowersPlaced}");
        }
        // Đếm riêng Ice Trap towers - KHÔNG đếm vào totalTowersPlaced
        else if (towerType == TowerCode.IceTrap)
        {
            iceTrapTowersPlaced++;
            Debug.Log($"Ice Trap đã được đặt! Tổng số Ice Trap: {iceTrapTowersPlaced}");
        }
        else if (towerType != TowerCode.NoTower)
        {
            // Chỉ đếm các tower khác vào totalTowersPlaced (không đếm NoTower, OneGunBarrel, IceTrap)
            totalTowersPlaced++;
            Debug.Log($"Tower {towerType} đã được đặt! Tổng số: {totalTowersPlaced}");
        }
        else
        {
            Debug.LogWarning("OnTowerPlaced được gọi với TowerCode.NoTower - có thể có lỗi!");
        }
        
        Debug.Log($"DEBUG: oneGunBarrelTowersPlaced hiện tại: {oneGunBarrelTowersPlaced}");
        Debug.Log($"DEBUG: iceTrapTowersPlaced hiện tại: {iceTrapTowersPlaced}");
        Debug.Log($"DEBUG: totalTowersPlaced hiện tại: {totalTowersPlaced}");
        Debug.Log("================================");
        
        // Cập nhật UI ngay lập tức khi đặt tower
        if (TowerQuestUI.Instance != null)
        {
            TowerQuestUI.Instance.UpdateQuestDisplay();
        }
        
        this.CheckQuests();
    }
    
    protected virtual void CheckQuests()
    {
        // Tạo bản sao của list để tránh lỗi khi thay đổi list trong khi duyệt
        var questsToCheck = new List<TowerQuest>(towerQuests);
        
        foreach (var quest in questsToCheck)
        {
            // Kiểm tra an toàn
            if (quest == null) continue;
            
            // QUAN TRỌNG: Kiểm tra quest chưa hoàn thành và đủ điều kiện
            if (!quest.isCompleted)
            {
                bool canComplete = false;
                
                // Quest tutorial di chuyển: kiểm tra movement tutorial
                if (quest.questName == "Movement Tutorial")
                {
                    canComplete = movementTutorialCompleted >= quest.requiredTowerCount;
                    Debug.Log($"MOVEMENT TUTORIAL CHECK: movementTutorialCompleted={movementTutorialCompleted}, required={quest.requiredTowerCount}, canComplete={canComplete}");
                    
                    if (canComplete)
                    {
                        Debug.Log($"Movement Tutorial có thể hoàn thành!");
                    }
                    else
                    {
                        Debug.Log($" Movement Tutorial chưa hoàn thành. Hãy di chuyển bằng WASD!");
                    }
                }
                // Quest đầu tiên: đếm tất cả towers
                else if (quest.questName == "Tower Builder I")
                {
                    canComplete = totalTowersPlaced >= quest.requiredTowerCount;
                }
                // Quest thứ hai: chỉ đếm OneGunBarrel towers
                else if (quest.questName == "Tower Builder II")
                {
                    canComplete = oneGunBarrelTowersPlaced >= quest.requiredTowerCount;
                    Debug.Log($" QUEST II CHECK: oneGunBarrelTowersPlaced={oneGunBarrelTowersPlaced}, required={quest.requiredTowerCount}, canComplete={canComplete}");
                    
                    if (canComplete)
                    {
                        Debug.Log($" Quest II có thể hoàn thành! Đã đặt đủ {oneGunBarrelTowersPlaced} OneGunBarrel towers");
                    }
                    else
                    {
                        Debug.Log($" Quest II chưa hoàn thành. Cần thêm {quest.requiredTowerCount - oneGunBarrelTowersPlaced} OneGunBarrel towers nữa");
                    }
                }
                // Quest thứ ba: chỉ đếm Ice Trap towers
                else if (quest.questName == "Tower Builder III")
                {
                    canComplete = iceTrapTowersPlaced >= quest.requiredTowerCount;
                    Debug.Log($" QUEST III CHECK: iceTrapTowersPlaced={iceTrapTowersPlaced}, required={quest.requiredTowerCount}, canComplete={canComplete}");
                    
                    if (canComplete)
                    {
                        Debug.Log($" Quest III có thể hoàn thành! Đã đặt đủ {iceTrapTowersPlaced} Ice Trap towers");
                    }
                    else
                    {
                        Debug.Log($" Quest III chưa hoàn thành. Cần thêm {quest.requiredTowerCount - iceTrapTowersPlaced} Ice Trap towers nữa");
                    }
                }
                // Final Mission: kiểm tra thời gian bảo vệ core
                else if (quest.questName == "Final Mission: Defend Core")
                {
                    canComplete = finalMissionCompleted;
                    Debug.Log($" FINAL MISSION CHECK: finalMissionCompleted={finalMissionCompleted}, canComplete={canComplete}");
                    
                    if (canComplete)
                    {
                        Debug.Log($" Final Mission có thể hoàn thành! Đã bảo vệ core thành công!");
                    }
                    else
                    {
                        Debug.Log($" Final Mission chưa hoàn thành. Hãy bảo vệ core trong 60 giây!");
                    }
                }
                // Các quest khác: giữ nguyên logic cũ
                else
                {
                    canComplete = totalTowersPlaced >= quest.requiredTowerCount;
                }
                
                if (canComplete)
                {
                    this.CompleteQuest(quest);
                }
            }
        }
    }
    
    protected virtual void CompleteQuest(TowerQuest quest)
    {
        // Kiểm tra an toàn
        if (quest == null)
        {
            Debug.LogWarning("CompleteQuest: quest bị null!");
            return;
        }
        
        quest.isCompleted = true;
        quest.isUnlocked = true; // QUAN TRỌNG: Set isUnlocked = true khi hoàn thành
        Debug.Log($"Hoàn thành nhiệm vụ: {quest.questName}!");
        Debug.Log($"Tower mới đã được mở khóa: {quest.unlockedTower}");
        
        // Tự động tạo quest mới khi hoàn thành quest
        if (quest.questName == "Movement Tutorial")
        {
            // Không cần tạo quest mới, chỉ hiển thị thông báo
            Debug.Log(" Movement Tutorial hoàn thành! MachineGun Tower đã được mở khóa!");
            Debug.Log(" Bắt đầu với Tower Builder I");
        }
        else if (quest.questName == "Tower Builder I")
        {
            this.CreateSecondQuest();
        }
        else if (quest.questName == "Tower Builder II")
        {
            this.CreateThirdQuest();
        }
        else if (quest.questName == "Tower Builder III")
        {
            this.CreateFinalMission();
        }
        else if (quest.questName == "Final Mission: Defend Core")
        {
            // Không cần tạo quest mới cho final mission
            Debug.Log(" Final Mission hoàn thành! Tutorial đã hoàn tất!");
        }
        
        // Hiển thị thông báo
        this.ShowQuestNotification(quest);
        
        // Hiển thị thông báo trên UI
        if (TowerQuestUI.Instance != null)
        {
            TowerQuestUI.Instance.ShowQuestCompletedNotification(quest.questName, quest.unlockedTower.ToString());
        }
        
        // Có thể thêm hiệu ứng âm thanh hoặc particle effect ở đây
    }
    
    protected virtual void ResetProgressForNewCycle()
    {
        Debug.Log("=== RESET TIẾN ĐỘ CHO CHU KỲ MỚI ===");
        
        // Reset counters
        totalTowersPlaced = 0;
        oneGunBarrelTowersPlaced = 0;
        iceTrapTowersPlaced = 0;
        movementTutorialCompleted = 0;
        finalMissionCompleted = false;
        
        // Reset tất cả quest về trạng thái ban đầu
        foreach (var quest in towerQuests)
        {
            if (quest != null)
            {
                quest.isCompleted = false;
                quest.isUnlocked = false;
            }
        }
        
        // Tạo lại quest đầu tiên
        this.InitializeQuests();
        
        Debug.Log("Đã reset tiến độ! Bắt đầu chu kỳ mới với quest 'Tower Builder I'");
        Debug.Log("================================");
        
        // Cập nhật UI
        if (TowerQuestUI.Instance != null)
        {
            TowerQuestUI.Instance.UpdateQuestDisplay();
        }
    }
    
    protected virtual void CreateSecondQuest()
    {
        // Kiểm tra xem đã có quest "Tower Builder II" chưa
        bool hasSecondQuest = towerQuests.Find(q => q.questName == "Tower Builder II") != null;
        
        if (!hasSecondQuest)
        {
            // KHÔNG reset oneGunBarrelTowersPlaced để giữ tiến độ cho quest thứ hai
            // Chỉ reset totalTowersPlaced để UI progress bar về 0
            totalTowersPlaced = 0;
            // oneGunBarrelTowersPlaced = 0; // ĐÃ LOẠI BỎ - giữ tiến độ cho quest thứ hai
            
            // Tự động tạo quest mới khi hoàn thành quest đầu tiên
            towerQuests.Add(new TowerQuest
            {
                questName = "Tower Builder II",
                description = "Place 2 OneGunBarrel towers to unlock Ice Trap",
                requiredTowerCount = 2,
                unlockedTower = TowerCode.IceTrap,
                isCompleted = false,
                isUnlocked = false
            });
            
            Debug.Log("Quest mới đã được tạo tự động: Tower Builder II");
            Debug.Log("UI Progress Bar đã reset về 0 để ghi nhận cho nhiệm vụ mới!");
            Debug.Log("Giữ nguyên tiến độ OneGunBarrel towers để hoàn thành quest thứ hai!");
            Debug.Log($"Hiện tại đã có {oneGunBarrelTowersPlaced} OneGunBarrel towers, cần thêm {2 - oneGunBarrelTowersPlaced} nữa!");
            Debug.Log($"DEBUG: Tổng số quest hiện tại: {towerQuests.Count}");
            
            // Debug chi tiết quest vừa tạo
            var newQuest = towerQuests[towerQuests.Count - 1];
            Debug.Log($"DEBUG: Quest mới - Name: {newQuest.questName}, Required: {newQuest.requiredTowerCount}, UnlockedTower: {newQuest.unlockedTower}");
            
            // Cập nhật UI để hiển thị quest mới và progress bar reset
            if (TowerQuestUI.Instance != null)
            {
                TowerQuestUI.Instance.UpdateQuestDisplay();
                
                // Hiển thị thông báo quest mới
                TowerQuestUI.Instance.ShowNewQuestNotification(
                    "Tower Builder II", 
                    "Place 2 OneGunBarrel towers to unlock Ice Trap"
                );
                
                Debug.Log(" UI đã được cập nhật với quest mới và thông báo!");
            }
        }
        else
        {
            Debug.Log("Quest 'Tower Builder II' đã tồn tại, không tạo mới");
        }
    }
    
    protected virtual void CreateThirdQuest()
    {
        // Kiểm tra xem đã có quest "Tower Builder III" chưa
        bool hasThirdQuest = towerQuests.Find(q => q.questName == "Tower Builder III") != null;
        
        if (!hasThirdQuest)
        {
            // KHÔNG reset iceTrapTowersPlaced để giữ tiến độ cho quest thứ ba
            // Chỉ reset totalTowersPlaced để UI progress bar về 0
            totalTowersPlaced = 0;
            
            // Tự động tạo quest mới khi hoàn thành quest thứ hai
            towerQuests.Add(new TowerQuest
            {
                questName = "Tower Builder III",
                description = "Place 4 Ice Trap towers to unlock Flame Trap",
                requiredTowerCount = 4,
                unlockedTower = TowerCode.FlameTrap,
                isCompleted = false,
                isUnlocked = false
            });
            
            Debug.Log("Quest mới đã được tạo tự động: Tower Builder III");
            Debug.Log("UI Progress Bar đã reset về 0 để ghi nhận cho nhiệm vụ mới!");
            Debug.Log("Giữ nguyên tiến độ Ice Trap towers để hoàn thành quest thứ ba!");
            Debug.Log($"Hiện tại đã có {iceTrapTowersPlaced} Ice Trap towers, cần thêm {4 - iceTrapTowersPlaced} nữa!");
            Debug.Log($"DEBUG: Tổng số quest hiện tại: {towerQuests.Count}");
            
            // Debug chi tiết quest vừa tạo
            var newQuest = towerQuests[towerQuests.Count - 1];
            Debug.Log($"DEBUG: Quest mới - Name: {newQuest.questName}, Required: {newQuest.requiredTowerCount}, UnlockedTower: {newQuest.unlockedTower}");
            
            // Cập nhật UI để hiển thị quest mới và progress bar reset
            if (TowerQuestUI.Instance != null)
            {
                TowerQuestUI.Instance.UpdateQuestDisplay();
                
                // Hiển thị thông báo quest mới
                TowerQuestUI.Instance.ShowNewQuestNotification(
                    "Tower Builder III", 
                    "Place 4 Ice Trap towers to unlock Flame Trap"
                );
                
                Debug.Log(" UI đã được cập nhật với quest mới và thông báo!");
            }
        }
        else
        {
            Debug.Log("Quest 'Tower Builder III' đã tồn tại, không tạo mới");
        }
    }
    
    protected virtual void CreateFinalMission()
    {
        // Kiểm tra xem có phải tutorial map không
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bool isTutorialMap = currentSceneName == "Hai_SampleScene";
        
        Debug.Log($"=== CREATING FINAL MISSION ===");
        Debug.Log($"Current scene: {currentSceneName}");
        Debug.Log($"Is tutorial map: {isTutorialMap}");
        
        if (isTutorialMap)
        {
            // Kiểm tra xem đã có final mission chưa
            bool hasFinalMission = towerQuests.Find(q => q.questName == "Final Mission: Defend Core") != null;
            
            if (!hasFinalMission)
            {
                // Tạo final mission cho tutorial map
                towerQuests.Add(new TowerQuest
                {
                    questName = "Final Mission: Defend Core",
                    description = "Defend the core for 60 seconds to complete the tutorial",
                    requiredTowerCount = 1, // Chỉ cần 1 để trigger (sẽ được check riêng)
                    unlockedTower = TowerCode.NoTower, // Không mở khóa tower mới
                    isCompleted = false,
                    isUnlocked = false
                });
                
                Debug.Log("Final Mission đã được tạo tự động sau Tower Builder III!");
                Debug.Log($"DEBUG: Tổng số quest hiện tại: {towerQuests.Count}");
                
                // Debug chi tiết quest vừa tạo
                var newQuest = towerQuests[towerQuests.Count - 1];
                Debug.Log($"DEBUG: Quest mới - Name: {newQuest.questName}, Required: {newQuest.requiredTowerCount}, UnlockedTower: {newQuest.unlockedTower}");
                
                // Cập nhật UI để hiển thị quest mới
                if (TowerQuestUI.Instance != null)
                {
                    TowerQuestUI.Instance.UpdateQuestDisplay();
                    
                    // Hiển thị thông báo quest mới
                    TowerQuestUI.Instance.ShowNewQuestNotification(
                        "Final Mission: Defend Core", 
                        "Defend the core for 60 seconds to complete the tutorial"
                    );
                    
                    Debug.Log("UI đã được cập nhật với Final Mission!");
                }
                
                // Bắt đầu countdown timer cho final mission
                this.StartFinalMissionCountdown();
            }
            else
            {
                Debug.Log("Final Mission đã tồn tại cho tutorial map");
            }
        }
        else
        {
            Debug.Log("Không phải tutorial map, không tạo Final Mission");
        }
        
        Debug.Log("================================");
    }
    
    protected virtual void StartFinalMissionCountdown()
    {
        try
        {
            Debug.Log("=== STARTING FINAL MISSION COUNTDOWN ===");
            
            // Kiểm tra GameResultManager có tồn tại không
            if (GameResultManager.Instance != null)
            {
                // Gọi method StartCountdownTimer từ GameResultManager
                GameResultManager.Instance.StartCountdownTimer();
                Debug.Log("Final Mission countdown started via GameResultManager!");
            }
            else
            {
                Debug.LogWarning("GameResultManager.Instance là null! Không thể bắt đầu countdown timer!");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong StartFinalMissionCountdown: {e.Message}");
        }
    }
    
    protected virtual void ShowQuestNotification(TowerQuest quest)
    {
        if (questNotificationPrefab != null)
        {
            // Tạo UI thông báo hoàn thành nhiệm vụ
            GameObject notification = Instantiate(questNotificationPrefab);
            // Có thể thêm logic hiển thị thông tin nhiệm vụ ở đây
        }
    }
    
    public virtual bool IsTowerUnlocked(TowerCode towerCode)
    {
        // Kiểm tra đặc biệt cho MachineGun - cần hoàn thành Movement Tutorial
        if (towerCode == TowerCode.MachineGun)
        {
            bool isUnlocked = movementTutorialCompleted >= 1;
            Debug.Log($"DEBUG: MachineGun Tower - Movement Tutorial Status: {movementTutorialCompleted}/1, IsUnlocked: {isUnlocked}");
            return isUnlocked;
        }
        
        // Kiểm tra xem tower có được mở khóa chưa
        foreach (var quest in towerQuests)
        {
            Debug.Log($"DEBUG: Checking quest '{quest.questName}' - UnlockedTower: {quest.unlockedTower}, IsCompleted: {quest.isCompleted}, IsUnlocked: {quest.isUnlocked}");
            
            if (quest.unlockedTower == towerCode && quest.isCompleted)
            {
                Debug.Log($"DEBUG: Tower {towerCode} đã được mở khóa bởi quest '{quest.questName}'");
                return true;
            }
        }
        
        Debug.Log($"DEBUG: Tower {towerCode} chưa được mở khóa");
        return false;
    }
    
    public virtual int GetTotalTowersPlaced()
    {
        return totalTowersPlaced;
    }
    
    public virtual int GetOneGunBarrelTowersPlaced()
    {
        return oneGunBarrelTowersPlaced;
    }
    
    public virtual int GetIceTrapTowersPlaced()
    {
        return iceTrapTowersPlaced;
    }
    
    /// <summary>
    /// Hoàn thành tutorial di chuyển
    /// </summary>
    public virtual void CompleteMovementTutorial()
    {
        Debug.Log(" CompleteMovementTutorial được gọi!");
        Debug.Log($" Trước khi hoàn thành: movementTutorialCompleted = {movementTutorialCompleted}");
        
        if (movementTutorialCompleted == 0)
        {
            movementTutorialCompleted = 1;
            Debug.Log(" Movement Tutorial đã hoàn thành!");
            Debug.Log(" MachineGun Tower đã được mở khóa!");
            Debug.Log($" Sau khi hoàn thành: movementTutorialCompleted = {movementTutorialCompleted}");
            
            // Cập nhật UI
            if (TowerQuestUI.Instance != null)
            {
                TowerQuestUI.Instance.UpdateQuestDisplay();
                Debug.Log(" UI đã được cập nhật!");
            }
            else
            {
                Debug.LogWarning(" TowerQuestUI.Instance là null!");
            }
            
            // Kiểm tra quest
            Debug.Log(" Bắt đầu kiểm tra quest...");
            this.CheckQuests();
        }
        else
        {
            Debug.Log($" Movement Tutorial đã hoàn thành trước đó (movementTutorialCompleted = {movementTutorialCompleted})");
        }
    }
    
    public virtual int GetMovementTutorialStatus()
    {
        return movementTutorialCompleted;
    }
    
    /// <summary>
    /// Kiểm tra xem có phải Map 1 không
    /// </summary>
    protected virtual bool IsMap1()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            // Map 1 của Bệ Hạ là "Hai_Map"
            return currentSceneName == "Hai_Map";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsMap1: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Hoàn thành final mission - bảo vệ core 60 giây
    /// </summary>
    public virtual void CompleteFinalMission()
    {
        Debug.Log(" CompleteFinalMission được gọi!");
        Debug.Log($" Trước khi hoàn thành: finalMissionCompleted = {finalMissionCompleted}");
        
        if (!finalMissionCompleted)
        {
            finalMissionCompleted = true;
            Debug.Log(" Final Mission đã hoàn thành! Đã bảo vệ core thành công!");
            Debug.Log($" Sau khi hoàn thành: finalMissionCompleted = {finalMissionCompleted}");
            
            // Cập nhật UI
            if (TowerQuestUI.Instance != null)
            {
                TowerQuestUI.Instance.UpdateQuestDisplay();
                Debug.Log(" UI đã được cập nhật!");
            }
            else
            {
                Debug.LogWarning(" TowerQuestUI.Instance là null!");
            }
            
            // Kiểm tra quest
            Debug.Log(" Bắt đầu kiểm tra quest...");
            this.CheckQuests();
        }
        else
        {
            Debug.Log($" Final Mission đã hoàn thành trước đó (finalMissionCompleted = {finalMissionCompleted})");
        }
    }
    
    public virtual bool GetFinalMissionStatus()
    {
        return finalMissionCompleted;
    }
    
    /// <summary>
    /// Test method để kiểm tra quest tutorial
    /// </summary>
    [ContextMenu("Test Movement Tutorial")]
    public virtual void TestMovementTutorial()
    {
        Debug.Log(" === TEST MOVEMENT TUTORIAL ===");
        Debug.Log($" movementTutorialCompleted: {movementTutorialCompleted}");
        Debug.Log($" Tổng số quest: {towerQuests.Count}");
        
        var movementQuest = towerQuests.Find(q => q.questName == "Movement Tutorial");
        if (movementQuest != null)
        {
            Debug.Log($" Tìm thấy Movement Tutorial quest:");
            Debug.Log($" - Required: {movementQuest.requiredTowerCount}");
            Debug.Log($" - Completed: {movementQuest.isCompleted}");
            Debug.Log($" - Can complete: {movementTutorialCompleted >= movementQuest.requiredTowerCount}");
        }
        else
        {
            Debug.LogError(" KHÔNG TÌM THẤY Movement Tutorial quest!");
        }
        
        Debug.Log(" === END TEST ===");
    }
    
    /// <summary>
    /// Test method để kiểm tra final mission
    /// </summary>
    [ContextMenu("Test Final Mission")]
    public virtual void TestFinalMission()
    {
        Debug.Log(" === TEST FINAL MISSION ===");
        Debug.Log($" finalMissionCompleted: {finalMissionCompleted}");
        Debug.Log($" Tổng số quest: {towerQuests.Count}");
        
        var finalMissionQuest = towerQuests.Find(q => q.questName == "Final Mission: Defend Core");
        if (finalMissionQuest != null)
        {
            Debug.Log($" Tìm thấy Final Mission quest:");
            Debug.Log($" - Required: {finalMissionQuest.requiredTowerCount}");
            Debug.Log($" - Completed: {finalMissionQuest.isCompleted}");
            Debug.Log($" - Can complete: {finalMissionCompleted}");
        }
        else
        {
            Debug.LogError(" KHÔNG TÌM THẤY Final Mission quest!");
        }
        
        Debug.Log(" === END TEST ===");
    }
    
    public virtual List<TowerQuest> GetActiveQuests()
    {
        // Trả về quest chưa hoàn thành, không phân biệt isUnlocked
        return towerQuests.FindAll(q => !q.isCompleted);
    }
    
    public virtual List<TowerQuest> GetAllQuests()
    {
        // Trả về tất cả quest, không phân biệt isUnlocked
        return new List<TowerQuest>(towerQuests);
    }
    
    public virtual void ResetQuests()
    {
        totalTowersPlaced = 0;
        oneGunBarrelTowersPlaced = 0; // Reset cả biến đếm OneGunBarrel
        iceTrapTowersPlaced = 0; // Reset cả biến đếm Ice Trap
        movementTutorialCompleted = 0; // Reset cả biến đếm tutorial di chuyển
        finalMissionCompleted = false; // Reset final mission
        foreach (var quest in towerQuests)
        {
            quest.isCompleted = false;
        }
        Debug.Log("Đã reset tất cả nhiệm vụ!");
    }
    
    public virtual void ResetAndReinitializeQuests()
    {
        // Xóa tất cả quest cũ
        towerQuests.Clear();
        
        // Tạo lại quest mới
        this.InitializeQuests();
        
        // Reset counter
        totalTowersPlaced = 0;
        oneGunBarrelTowersPlaced = 0; // Reset cả biến đếm OneGunBarrel
        iceTrapTowersPlaced = 0; // Reset cả biến đếm Ice Trap
        movementTutorialCompleted = 0; // Reset cả biến đếm tutorial di chuyển
        finalMissionCompleted = false; // Reset final mission
        
        Debug.Log("Đã reset và tạo lại tất cả nhiệm vụ!");
    }
    
    public virtual void RemoveQuestByName(string questName)
    {
        int removedCount = towerQuests.RemoveAll(q => q.questName == questName);
        Debug.Log($"Đã xóa {removedCount} quest có tên '{questName}'");
    }
    
    public virtual void RemoveQuestByUnlockedTower(TowerCode towerCode)
    {
        int removedCount = towerQuests.RemoveAll(q => q.unlockedTower == towerCode);
        Debug.Log($"Đã xóa {removedCount} quest mở khóa tower '{towerCode}'");
    }
    
    public virtual void ListAllQuests()
    {
        Debug.Log("=== DANH SÁCH TẤT CẢ QUEST ===");
        for (int i = 0; i < towerQuests.Count; i++)
        {
            var quest = towerQuests[i];
            Debug.Log($"{i + 1}. '{quest.questName}' - Mở khóa: {quest.unlockedTower} - Hoàn thành: {quest.isCompleted}");
        }
        Debug.Log($"Tổng số quest: {towerQuests.Count}");
        Debug.Log($"Tổng số towers đã đặt: {totalTowersPlaced}");
        Debug.Log($"Số OneGunBarrel towers đã đặt: {oneGunBarrelTowersPlaced}");
        Debug.Log($"Số Ice Trap towers đã đặt: {iceTrapTowersPlaced}");
        Debug.Log($"Movement Tutorial: {movementTutorialCompleted}/1");
        Debug.Log($"Final Mission: {(finalMissionCompleted ? "Completed" : "Not Completed")}");
        Debug.Log("================================");
    }
    
    public virtual void DebugQuestStatus()
    {
        Debug.Log("=== DEBUG QUEST STATUS ===");
        Debug.Log($"Tổng số quest: {towerQuests.Count}");
        Debug.Log($"Tổng số towers đã đặt: {totalTowersPlaced}");
        Debug.Log($"Số OneGunBarrel towers đã đặt: {oneGunBarrelTowersPlaced}");
        Debug.Log($"Số Ice Trap towers đã đặt: {iceTrapTowersPlaced}");
        Debug.Log($"Movement Tutorial: {movementTutorialCompleted}/1 - MachineGun Unlocked: {(movementTutorialCompleted >= 1 ? "Yes" : "No")}");
        Debug.Log($"Final Mission: {(finalMissionCompleted ? "Completed" : "Not Completed")}");
        
        foreach (var quest in towerQuests)
        {
            if (quest != null)
            {
                Debug.Log($"Quest: '{quest.questName}' - Completed: {quest.isCompleted} - Required: {quest.requiredTowerCount}");
                if (quest.questName == "Movement Tutorial")
                {
                    bool canComplete = movementTutorialCompleted >= quest.requiredTowerCount;
                    Debug.Log($" Movement Tutorial can complete: {canComplete} (Status: {movementTutorialCompleted}/{quest.requiredTowerCount})");
                    
                    if (canComplete)
                    {
                        Debug.Log($" Movement Tutorial sẵn sàng hoàn thành!");
                    }
                    else
                    {
                        Debug.Log($" Movement Tutorial chưa hoàn thành. Hãy di chuyển bằng WASD!");
                    }
                }
                else if (quest.questName == "Tower Builder II")
                {
                    bool canComplete = oneGunBarrelTowersPlaced >= quest.requiredTowerCount;
                    Debug.Log($" Quest II can complete: {canComplete} (OneGunBarrel: {oneGunBarrelTowersPlaced}/{quest.requiredTowerCount})");
                    
                    if (canComplete)
                    {
                        Debug.Log($" Quest II sẵn sàng hoàn thành!");
                    }
                    else
                    {
                        Debug.Log($" Quest II cần thêm {quest.requiredTowerCount - oneGunBarrelTowersPlaced} OneGunBarrel towers");
                    }
                }
                else if (quest.questName == "Tower Builder III")
                {
                    bool canComplete = iceTrapTowersPlaced >= quest.requiredTowerCount;
                    Debug.Log($" Quest III can complete: {canComplete} (Ice Trap: {iceTrapTowersPlaced}/{quest.requiredTowerCount})");
                    
                    if (canComplete)
                    {
                        Debug.Log($" Quest III sẵn sàng hoàn thành!");
                    }
                    else
                    {
                        Debug.Log($" Quest III cần thêm {quest.requiredTowerCount - iceTrapTowersPlaced} Ice Trap towers");
                    }
                }
                else if (quest.questName == "Final Mission: Defend Core")
                {
                    bool canComplete = finalMissionCompleted;
                    Debug.Log($" Final Mission can complete: {canComplete} (Status: {(finalMissionCompleted ? "Completed" : "Not Completed")})");
                    
                    if (canComplete)
                    {
                        Debug.Log($" Final Mission sẵn sàng hoàn thành!");
                    }
                    else
                    {
                        Debug.Log($" Final Mission chưa hoàn thành. Hãy bảo vệ core trong 60 giây!");
                    }
                }
            }
        }
        
        // Debug UI Progress
        if (TowerQuestUI.Instance != null)
        {
            Debug.Log("=== UI PROGRESS DEBUG ===");
            var activeQuests = this.GetActiveQuests();
            if (activeQuests.Count > 0)
            {
                var currentQuest = activeQuests[0];
                if (currentQuest.questName == "Movement Tutorial")
                {
                    Debug.Log($" UI Movement Tutorial Progress: {movementTutorialCompleted}/{currentQuest.requiredTowerCount}");
                }
                else if (currentQuest.questName == "Tower Builder II")
                {
                    Debug.Log($" UI Quest II Progress: {oneGunBarrelTowersPlaced}/{currentQuest.requiredTowerCount}");
                }
                else if (currentQuest.questName == "Tower Builder III")
                {
                    Debug.Log($" UI Quest III Progress: {iceTrapTowersPlaced}/{currentQuest.requiredTowerCount}");
                }
                else if (currentQuest.questName == "Final Mission: Defend Core")
                {
                    Debug.Log($" UI Final Mission Progress: {(finalMissionCompleted ? "Completed" : "Not Completed")}");
                }
                else
                {
                    Debug.Log($" UI Quest {currentQuest.questName} Progress: {totalTowersPlaced}/{currentQuest.requiredTowerCount}");
                }
            }
        }
        
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Kiểm tra xem có phải scene hợp lệ không (chỉ Hai_SampleScene)
    /// </summary>
    protected virtual bool IsValidScene()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            bool isValid = currentSceneName == "Hai_SampleScene";
            Debug.Log($"TowerQuestSystem: Current scene '{currentSceneName}', IsValid: {isValid}");
            return isValid;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsValidScene: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Destroy instance khi không ở scene hợp lệ
    /// </summary>
    protected virtual void DestroyInstance()
    {
        if (this.isDestroyed) return;
        
        Debug.Log("TowerQuestSystem: Destroying instance - not in Hai_SampleScene");
        this.isDestroyed = true;
        
        // Clear all data
        towerQuests.Clear();
        totalTowersPlaced = 0;
        oneGunBarrelTowersPlaced = 0;
        iceTrapTowersPlaced = 0;
        movementTutorialCompleted = 0;
        finalMissionCompleted = false;
        
        // Destroy GameObject
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Kiểm tra xem instance có bị destroy không
    /// </summary>
    public virtual bool IsDestroyed()
    {
        return this.isDestroyed;
    }

} 