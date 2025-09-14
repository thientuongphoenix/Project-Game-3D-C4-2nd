using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Thêm để sử dụng CanvasGroup

public class GameResultManager : SaiSingleton<GameResultManager>
{
    protected override void Start()
    {
        base.Start();
        this.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameEnded) return;
        this.UpdateTimer();
        this.CheckLoseCondition();
        this.CheckWinCondition();
    }

    
    [SerializeField] protected float timer = 0f;
    [SerializeField] protected bool isGameEnded = false;
    [SerializeField] protected bool isLose = false;
    [SerializeField] protected bool isWin = false;

    [SerializeField] protected PlayerCtrl player;
    [SerializeField] protected CoreCtrl core;
    [SerializeField] protected GameObject winPanel;
    [SerializeField] protected GameObject losePanel;
    [SerializeField] protected float defendTime = 60f;
    
    // Thêm biến cho hiệu ứng fade
    [SerializeField] protected float fadeInDuration = 3f;
    protected CanvasGroup winPanelCanvasGroup;
    protected CanvasGroup losePanelCanvasGroup;

    protected virtual void Init()
    {
        // Reset trạng thái game
        this.ResetGameState();
        
        this.player = PlayerCtrl.Instance;
        this.core = FindObjectOfType<CoreCtrl>();
        this.winPanel = GameObject.Find("WinPanel");
        this.losePanel = GameObject.Find("LosePanel");
        
        // Khởi tạo CanvasGroup cho fade effect
        this.InitializeCanvasGroups();
        
        if (this.winPanel != null) this.winPanel.SetActive(false);
        if (this.losePanel != null) this.losePanel.SetActive(false);
        
        // Reset nhiệm vụ khi vào lại tutorial map nếu map 1 đã được unlock
        this.CheckAndResetTutorialQuests();
        
        Debug.Log("GameResultManager: Initialized for new scene");
    }
    
    protected virtual void ResetGameState()
    {
        this.timer = 0f;
        this.isGameEnded = false;
        this.isLose = false;
        this.isWin = false;
    }
    
    protected virtual void InitializeCanvasGroups()
    {
        // Khởi tạo CanvasGroup cho WinPanel
        if (this.winPanel != null)
        {
            this.winPanelCanvasGroup = this.winPanel.GetComponent<CanvasGroup>();
            if (this.winPanelCanvasGroup == null)
            {
                this.winPanelCanvasGroup = this.winPanel.AddComponent<CanvasGroup>();
            }
            this.winPanelCanvasGroup.alpha = 0f;
        }
        
        // Khởi tạo CanvasGroup cho LosePanel
        if (this.losePanel != null)
        {
            this.losePanelCanvasGroup = this.losePanel.GetComponent<CanvasGroup>();
            if (this.losePanelCanvasGroup == null)
            {
                this.losePanelCanvasGroup = this.losePanel.AddComponent<CanvasGroup>();
            }
            this.losePanelCanvasGroup.alpha = 0f;
        }
    }

    protected virtual void UpdateTimer()
    {
        // Chỉ update timer cho map 1, không dùng cho tutorial map
        if (IsMap1())
        {
            timer += Time.deltaTime;
        }
    }
    
    // Method để kiểm tra xem có phải map 1 không
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
    
    // Method để kiểm tra xem có phải tutorial map không
    protected virtual bool IsTutorialMap()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            return currentSceneName == "Hai_SampleScene";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsTutorialMap: {e.Message}");
            return false;
        }
    }
    
    // Method để kiểm tra và reset nhiệm vụ tutorial
    protected virtual void CheckAndResetTutorialQuests()
    {
        try
        {
            // Chỉ reset khi đang ở tutorial map
            if (!IsTutorialMap()) return;
            
            Debug.Log("=== CHECKING TUTORIAL QUEST RESET ===");
            
            // Kiểm tra xem map 1 đã được unlock chưa
            bool isMap1Unlocked = IsMap1Unlocked();
            Debug.Log($"Map 1 unlocked: {isMap1Unlocked}");
            
            if (isMap1Unlocked)
            {
                // Nếu map 1 đã unlock, reset lại tất cả nhiệm vụ tutorial
                ResetTutorialQuests();
                Debug.Log("Tutorial quests have been reset!");
            }
            else
            {
                Debug.Log("Map 1 not unlocked yet, keeping tutorial quests as is");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong CheckAndResetTutorialQuests: {e.Message}");
        }
    }
    
    // Method để kiểm tra xem tutorial map đã được completed chưa (tức là map 1 đã unlock)
    protected virtual bool IsMap1Unlocked()
    {
        try
        {
            if (MapProgressManager.Instance != null)
            {
                // Kiểm tra xem tutorial map đã được completed chưa
                // Nếu tutorial map đã completed thì map 1 đã được unlock
                bool isUnlocked = MapProgressManager.Instance.IsMapCompleted("Hai_SampleScene");
                Debug.Log($"Tutorial map completed (Map 1 unlocked): {isUnlocked}");
                return isUnlocked;
            }
            else
            {
                Debug.LogWarning("MapProgressManager.Instance is null, cannot check map unlock status");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsMap1Unlocked: {e.Message}");
            return false;
        }
    }
    
    // Method để reset tất cả nhiệm vụ tutorial
    protected virtual void ResetTutorialQuests()
    {
        try
        {
            if (TowerQuestSystem.Instance != null)
            {
                Debug.Log("=== RESETTING TUTORIAL QUESTS ===");
                
                // Sử dụng method ResetAndReinitializeQuests() có sẵn trong TowerQuestSystem
                // Method này sẽ reset cả quests và progress counters
                TowerQuestSystem.Instance.ResetAndReinitializeQuests();
                
                Debug.Log("Tutorial quests and progress have been completely reset!");
                Debug.Log("All quests are now in initial state and ready to be completed again");
                
                // Cập nhật UI để hiển thị trạng thái mới
                if (TowerQuestUI.Instance != null)
                {
                    TowerQuestUI.Instance.UpdateQuestDisplay();
                    Debug.Log("Quest UI updated with reset state");
                }
            }
            else
            {
                Debug.LogWarning("TowerQuestSystem.Instance is null, cannot reset quests");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong ResetTutorialQuests: {e.Message}");
        }
    }
    
    // Method để kiểm tra xem Final Mission có đang active không
    protected virtual bool IsFinalMissionActive()
    {
        try
        {
            if (IsTutorialMap() && TowerQuestSystem.Instance != null)
            {
                var finalMissionQuest = TowerQuestSystem.Instance.GetAllQuests().Find(q => q.questName == "Final Mission: Defend Core");
                bool hasFinalMission = finalMissionQuest != null && !finalMissionQuest.isCompleted;
                
                if (hasFinalMission && CountdownTimerUI.Instance != null)
                {
                    return CountdownTimerUI.Instance.IsFinalMission();
                }
            }
            
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsFinalMissionActive: {e.Message}");
            return false;
        }
    }

    protected virtual void CheckLoseCondition()
    {
        if (isLose || isGameEnded) return;
        if (IsPlayerDead() || IsCoreDead())
        {
            isLose = true;
            // Tắt tất cả SFX ngay lập tức, giữ lại nhạc
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.VolumeSfxUpdating(0f);
                //Debug.Log("Đã tắt tất cả SFX, giữ lại nhạc nền");
            }
            StartCoroutine(ShowLosePanelAfterDelay(2f));
        }
    }

    protected virtual void CheckWinCondition()
    {
        if (isWin || isGameEnded) return;
        
        // Xử lý win condition cho tutorial map
        if (IsTutorialMap())
        {
            // Chỉ check win condition khi Final Mission đang active
            if (IsFinalMissionActive())
            {
                // Countdown timer sẽ tự xử lý win condition
                return;
            }
            
            // Nếu không có Final Mission active, không có win condition
            return;
        }
        
        // Xử lý win condition cho map 1
        if (IsMap1())
        {
            if (timer >= defendTime)
            {
                isWin = true;
                // Tắt tất cả SFX ngay lập tức, giữ lại nhạc
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.VolumeSfxUpdating(0f);
                    Debug.Log("Map 1: Đã tắt tất cả SFX, giữ lại nhạc nền");
                }
                
                ShowWinPanel();
            }
        }
    }

    protected virtual bool IsPlayerDead()
    {
        if (player == null || player.PlayerDamageReceiver == null) return false;
        return player.PlayerDamageReceiver.IsDead();
    }

    protected virtual bool IsCoreDead()
    {
        if (core == null || core.TowerDamageReceiver == null) return false;
        return core.TowerDamageReceiver.IsDead();
    }

    protected virtual IEnumerator ShowLosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowLosePanel();
    }

    protected virtual void ShowWinPanel()
    {
        Debug.Log("=== ShowWinPanel() called ===");
        isGameEnded = true;
        
        // Ẩn quest panel khi hiển thị win panel
        this.HideQuestPanel();
        
        if (winPanel != null) 
        {
            Debug.Log("Win panel found, activating...");
            winPanel.SetActive(true);
            StartCoroutine(FadeInPanel(winPanelCanvasGroup));
            Debug.Log("Win panel activated successfully!");
        }
        else
        {
            Debug.LogError("Win panel is NULL! Cannot show win panel!");
        }
        
        if (losePanel != null) losePanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        
        // Hoàn thành map hiện tại
        this.CompleteCurrentMap();
        
        Debug.Log("=== ShowWinPanel() completed ===");
        // Có thể bổ sung hiệu ứng, âm thanh,... ở đây
    }
    
    protected virtual void CompleteCurrentMap()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"=== COMPLETING MAP: {currentSceneName} ===");
        
        // Thử tìm MapProgressManager nếu Instance bị null
        if (MapProgressManager.Instance == null)
        {
            Debug.LogWarning("MapProgressManager.Instance is NULL! Attempting to find MapProgressManager...");
            this.TryFindMapProgressManager();
            
            // Đợi một frame để MapProgressManager khởi tạo xong
            StartCoroutine(CompleteMapAfterDelay(currentSceneName));
            return;
        }
        
        // Nếu MapProgressManager có sẵn, xử lý ngay
        this.ProcessMapCompletion(currentSceneName);
    }
    
    protected virtual void TryFindMapProgressManager()
    {
        // Tìm MapProgressManager trong scene
        MapProgressManager mapProgressManager = FindObjectOfType<MapProgressManager>();
        if (mapProgressManager != null)
        {
            Debug.Log("MapProgressManager found in scene! Attempting to initialize...");
            // Có thể cần khởi tạo lại Instance nếu có method public
            // mapProgressManager.InitializeInstance(); // Nếu có method này
        }
        else
        {
            Debug.LogWarning("MapProgressManager not found in scene! Creating new one...");
            this.CreateMapProgressManager();
        }
    }
    
    protected virtual void CreateMapProgressManager()
    {
        try
        {
            Debug.Log("Creating new MapProgressManager...");
            
            // Tạo GameObject mới cho MapProgressManager
            GameObject mapProgressManagerGO = new GameObject("MapProgressManager");
            
            // Thêm MapProgressManager component
            MapProgressManager newMapProgressManager = mapProgressManagerGO.AddComponent<MapProgressManager>();
            
            // Đặt DontDestroyOnLoad để không bị destroy khi chuyển scene
            DontDestroyOnLoad(mapProgressManagerGO);
            
            Debug.Log("MapProgressManager created successfully!");
            
            // Kiểm tra xem Instance đã được set chưa
            if (MapProgressManager.Instance == null)
            {
                Debug.LogWarning("MapProgressManager.Instance is still null after creation. This might be a singleton issue.");
            }
            else
            {
                Debug.Log("MapProgressManager.Instance is now available!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating MapProgressManager: {e.Message}");
        }
    }
    
    protected virtual void TryUnlockNextMap(string currentMapName)
    {
        Debug.Log($"=== TRYING TO UNLOCK NEXT MAP ===");
        Debug.Log($"Current map: {currentMapName}");
        
        try
        {
            // Kiểm tra xem có phải tutorial map không
            if (currentMapName == "Hai_SampleScene")
            {
                Debug.Log("Tutorial map completed! Attempting to unlock next map...");
                
                // Có thể cần gọi method unlock map tiếp theo
                // Ví dụ: MapProgressManager.Instance.UnlockNextMap();
                // Hoặc: MapProgressManager.Instance.UnlockMap("Map2");
                
                // Kiểm tra xem có method unlock không
                var mapProgressManager = MapProgressManager.Instance;
                if (mapProgressManager != null)
                {
                    // Thử gọi method unlock nếu có
                    var unlockMethod = mapProgressManager.GetType().GetMethod("UnlockNextMap");
                    if (unlockMethod != null)
                    {
                        unlockMethod.Invoke(mapProgressManager, null);
                        Debug.Log("UnlockNextMap() method called successfully!");
                    }
                    else
                    {
                        Debug.LogWarning("UnlockNextMap() method not found in MapProgressManager!");
                    }
                    
                    // Thử gọi method unlock map cụ thể
                    var unlockMapMethod = mapProgressManager.GetType().GetMethod("UnlockMap");
                    if (unlockMapMethod != null)
                    {
                        // Có thể cần thay đổi tên map tùy theo game
                        unlockMapMethod.Invoke(mapProgressManager, new object[] { "Map2" });
                        Debug.Log("UnlockMap(Map2) method called successfully!");
                    }
                    else
                    {
                        Debug.LogWarning("UnlockMap() method not found in MapProgressManager!");
                    }
                }
            }
            else
            {
                Debug.Log($"Not tutorial map ({currentMapName}), no special unlock needed");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in TryUnlockNextMap: {e.Message}");
        }
        
        Debug.Log("================================");
    }
    
    protected virtual IEnumerator CompleteMapAfterDelay(string currentSceneName)
    {
        // Đợi một frame để MapProgressManager khởi tạo xong
        yield return null;
        
        Debug.Log("Retrying map completion after MapProgressManager creation...");
        this.ProcessMapCompletion(currentSceneName);
    }
    
    protected virtual void ProcessMapCompletion(string currentSceneName)
    {
        if (MapProgressManager.Instance != null)
        {
            Debug.Log($"MapProgressManager found! Completing map: {currentSceneName}");
            
            // Lưu map hiện tại
            MapProgressManager.Instance.CompleteMap(currentSceneName);
            Debug.Log($"Map {currentSceneName} completed and saved!");
            
            // Debug: Kiểm tra danh sách completed maps trước khi unlock
            var completedMapsBefore = MapProgressManager.Instance.GetCompletedMaps();
            Debug.Log($"Completed maps BEFORE unlock: {string.Join(", ", completedMapsBefore)}");
            
            // Thử unlock map tiếp theo
            this.TryUnlockNextMap(currentSceneName);
            
            // Debug: Kiểm tra danh sách completed maps sau khi unlock
            var completedMapsAfter = MapProgressManager.Instance.GetCompletedMaps();
            Debug.Log($"Completed maps AFTER unlock: {string.Join(", ", completedMapsAfter)}");
        }
        else
        {
            Debug.LogError("MapProgressManager.Instance is still NULL after creation! Cannot save map progress or unlock next map!");
            // Không chặn game, chỉ cảnh báo và tiếp tục
        }
    }

    protected virtual void ShowLosePanel()
    {
        isGameEnded = true;
        
        // Ẩn quest panel khi hiển thị lose panel
        this.HideQuestPanel();
        
        if (losePanel != null) 
        {
            losePanel.SetActive(true);
            StartCoroutine(FadeInPanel(losePanelCanvasGroup));
        }
        if (winPanel != null) winPanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        // Có thể bổ sung hiệu ứng, âm thanh,... ở đây
    }
    
    // Thêm hàm fade in cho panel
    protected virtual IEnumerator FadeInPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) yield break;
        
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        
        
    }
    
    protected virtual void CheckAndCompleteFinalMission()
    {
        try
        {
            // Kiểm tra xem có phải tutorial map không
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            bool isTutorialMap = currentSceneName == "Hai_SampleScene";
            
            Debug.Log($"=== CHECKING FINAL MISSION COMPLETION ===");
            Debug.Log($"Current scene: {currentSceneName}");
            Debug.Log($"Is tutorial map: {isTutorialMap}");
            
            if (isTutorialMap)
            {
                // Kiểm tra TowerQuestSystem có tồn tại không
                if (TowerQuestSystem.Instance != null)
                {
                    // Hoàn thành final mission
                    TowerQuestSystem.Instance.CompleteFinalMission();
                    Debug.Log("Final Mission đã được hoàn thành! Đã bảo vệ core thành công trong 60 giây!");
                }
                else
                {
                    Debug.LogWarning("TowerQuestSystem.Instance là null! Không thể hoàn thành final mission!");
                }
            }
            else
            {
                Debug.Log("Không phải tutorial map, không cần hoàn thành final mission");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong CheckAndCompleteFinalMission: {e.Message}");
        }
    }
    
    public virtual void StartCountdownTimer()
    {
        try
        {
            Debug.Log($"=== STARTING COUNTDOWN TIMER ===");
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            Debug.Log($"Is tutorial map: {IsTutorialMap()}");
            Debug.Log($"Is map 1: {IsMap1()}");
            
            if (IsTutorialMap())
            {
                // Kiểm tra TowerQuestSystem có tồn tại không
                if (TowerQuestSystem.Instance != null)
                {
                    // Kiểm tra xem có final mission không
                    var finalMissionQuest = TowerQuestSystem.Instance.GetAllQuests().Find(q => q.questName == "Final Mission: Defend Core");
                    
                    if (finalMissionQuest != null && !finalMissionQuest.isCompleted)
                    {
                        // Bắt đầu countdown timer
                        if (CountdownTimerUI.Instance != null)
                        {
                            CountdownTimerUI.Instance.StartCountdown(this.defendTime);
                            Debug.Log($"Tutorial map: Countdown timer started for {this.defendTime} seconds! Defend time is now active!");
                        }
                        else
                        {
                            Debug.LogWarning("CountdownTimerUI.Instance là null! Không thể bắt đầu countdown timer!");
                        }
                    }
                    else
                    {
                        Debug.Log("Final Mission chưa có hoặc đã hoàn thành, không bắt đầu countdown timer");
                    }
                }
                else
                {
                    Debug.LogWarning("TowerQuestSystem.Instance là null! Không thể kiểm tra final mission!");
                }
            }
            else if (IsMap1())
            {
                Debug.Log("Map 1: Sử dụng timer cũ, không cần countdown timer");
            }
            else
            {
                Debug.Log("Không phải tutorial map hoặc map 1, không bắt đầu countdown timer");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong StartCountdownTimer: {e.Message}");
        }
    }
    
    protected virtual void StopCountdownTimer()
    {
        try
        {
            if (CountdownTimerUI.Instance != null)
            {
                CountdownTimerUI.Instance.StopCountdown();
                Debug.Log("Countdown timer stopped!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong StopCountdownTimer: {e.Message}");
        }
    }
    
    public virtual void OnFinalMissionCompleted()
    {
        try
        {
            Debug.Log("=== FINAL MISSION COMPLETED ===");
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            
            isWin = true;
            
            // Tắt tất cả SFX ngay lập tức, giữ lại nhạc
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.VolumeSfxUpdating(0f);
                Debug.Log("Đã tắt tất cả SFX, giữ lại nhạc nền");
            }
            
            // Dừng countdown timer
            this.StopCountdownTimer();
            
            // Hoàn thành final mission
            this.CheckAndCompleteFinalMission();
            
            // Hiển thị win panel
            Debug.Log("About to call ShowWinPanel()...");
            ShowWinPanel();
            
            Debug.Log("Final Mission completed successfully!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong OnFinalMissionCompleted: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Ẩn quest panel và countdown timer khi game kết thúc
    /// </summary>
    protected virtual void HideQuestPanel()
    {
        try
        {
            Debug.Log("Hiding quest panel and countdown timer...");
            
            // Ẩn quest panel
            this.HideQuestUI();
            
            // Ẩn countdown timer
            this.HideCountdownTimer();
            
            Debug.Log("Quest panel and countdown timer hidden successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error hiding quest panel and countdown timer: {e.Message}");
        }
    }
    
    /// <summary>
    /// Ẩn quest UI
    /// </summary>
    protected virtual void HideQuestUI()
    {
        try
        {
            // Tìm quest panel trong scene
            GameObject questPanel = GameObject.Find("QuestPanel");
            if (questPanel == null)
            {
                // Thử tìm với tên khác
                questPanel = GameObject.Find("QuestUI");
            }
            if (questPanel == null)
            {
                // Thử tìm với tên khác
                questPanel = GameObject.Find("QuestCanvas");
            }
            if (questPanel == null)
            {
                // Thử tìm TowerQuestSystem UI
                var towerQuestSystem = FindObjectOfType<TowerQuestSystem>();
                if (towerQuestSystem != null)
                {
                    questPanel = towerQuestSystem.gameObject;
                }
            }
            
            if (questPanel != null)
            {
                questPanel.SetActive(false);
                Debug.Log("Quest panel hidden successfully!");
            }
            else
            {
                Debug.LogWarning("Quest panel not found! Quest UI might have different name.");
                
                // Thử ẩn tất cả UI có chứa "quest" trong tên
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("quest") && obj.activeInHierarchy)
                    {
                        obj.SetActive(false);
                        Debug.Log($"Hidden quest-related object: {obj.name}");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error hiding quest UI: {e.Message}");
        }
    }
    
    /// <summary>
    /// Ẩn countdown timer UI
    /// </summary>
    protected virtual void HideCountdownTimer()
    {
        try
        {
            // Tìm countdown timer UI
            GameObject countdownTimer = GameObject.Find("CountdownTimer");
            if (countdownTimer == null)
            {
                // Thử tìm với tên khác
                countdownTimer = GameObject.Find("CountdownTimerUI");
            }
            if (countdownTimer == null)
            {
                // Thử tìm với tên khác
                countdownTimer = GameObject.Find("TimerUI");
            }
            if (countdownTimer == null)
            {
                // Thử tìm CountdownTimerUI component
                var countdownTimerComponent = FindObjectOfType<CountdownTimerUI>();
                if (countdownTimerComponent != null)
                {
                    countdownTimer = countdownTimerComponent.gameObject;
                }
            }
            
            if (countdownTimer != null)
            {
                countdownTimer.SetActive(false);
                Debug.Log("Countdown timer hidden successfully!");
            }
            else
            {
                Debug.LogWarning("Countdown timer not found! Timer UI might have different name.");
                
                // Thử ẩn tất cả UI có chứa "timer" hoặc "countdown" trong tên
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    string objName = obj.name.ToLower();
                    if ((objName.Contains("timer") || objName.Contains("countdown")) && obj.activeInHierarchy)
                    {
                        obj.SetActive(false);
                        Debug.Log($"Hidden timer-related object: {obj.name}");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error hiding countdown timer: {e.Message}");
        }
    }
}
