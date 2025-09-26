using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// GameResultManager riêng cho Map 1 để tránh xung đột với Tutorial Map
/// </summary>
public class Map1GameResultManager : SaiSingleton<Map1GameResultManager>
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
    
    // Thêm biến để theo dõi coroutine
    protected Coroutine winPanelFadeCoroutine;
    protected Coroutine losePanelFadeCoroutine;

    protected virtual void Init()
    {
        Debug.Log("=== MAP1 GAMERESULTMANAGER INIT START ===");
        Debug.Log($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"isGameEnded before reset: {isGameEnded}");
        
        // Reset trạng thái game cho Map 1
        this.ResetGameState();
        
        Debug.Log($"isGameEnded after reset: {isGameEnded}");
        
        this.player = PlayerCtrl.Instance;
        this.core = FindObjectOfType<CoreCtrl>();
        
        // Reset panel references khi vào scene mới
        this.winPanel = null;
        this.losePanel = null;
        
        // Ẩn tất cả Win/Lose panel trước khi tìm mới
        this.HideAllWinLosePanelsInScene();
        
        // Tìm WinPanel và LosePanel trong scene hiện tại
        this.FindWinLosePanelsInCurrentScene();
        
        // Khởi tạo CanvasGroup cho fade effect
        this.InitializeCanvasGroups();
        
        // Đảm bảo panel bị ẩn khi start game
        if (this.winPanel != null) this.winPanel.SetActive(false);
        if (this.losePanel != null) this.losePanel.SetActive(false);
        
        // Đảm bảo sound system được khởi tạo
        this.EnsureSoundSystemInMap1();
        
        // Ẩn QuestPanel từ SampleScene nếu có
        this.HideQuestPanelFromSampleScene();
        
        Debug.Log($"Map1GameResultManager: Initialized for {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"WinPanel found: {winPanel != null}");
        Debug.Log($"LosePanel found: {losePanel != null}");
        Debug.Log($"isGameEnded final: {isGameEnded}");
        Debug.Log("=== MAP1 GAMERESULTMANAGER INIT END ===");
    }
    
    protected virtual void ResetGameState()
    {
        Debug.Log("=== MAP1 RESET GAME STATE ===");
        Debug.Log($"isGameEnded before reset: {isGameEnded}");
        
        this.timer = 0f;
        this.isGameEnded = false;
        this.isLose = false;
        this.isWin = false;
        
        Debug.Log($"isGameEnded after reset: {isGameEnded}");
        
        // Dừng tất cả coroutine fade
        this.StopAllFadeCoroutines();
        
        // Hiển thị lại TowerInfoUI khi game bắt đầu
        this.ShowTowerInfoUI();
        
        Debug.Log("=== MAP1 RESET GAME STATE COMPLETED ===");
    }
    
    protected virtual void UpdateTimer()
    {
        if (!isGameEnded)
        {
            timer += Time.deltaTime;
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
            }
            StartCoroutine(ShowLosePanelAfterDelay(2f));
        }
    }

    protected virtual void CheckWinCondition()
    {
        if (isWin || isGameEnded) return;
        
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
    
    protected virtual bool IsMap1()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            return currentSceneName == "Hai_Map";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsMap1: {e.Message}");
            return false;
        }
    }
    
    protected virtual IEnumerator ShowLosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowLosePanel();
    }

    protected virtual void ShowWinPanel()
    {
        Debug.Log("=== MAP1 ShowWinPanel() called ===");
        Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"isGameEnded before: {isGameEnded}");
        
        isGameEnded = true;
        Debug.Log($"isGameEnded after: {isGameEnded}");
        
        // Ẩn quest panel khi hiển thị win panel
        this.HideQuestPanel();
        
        // Ẩn TowerInfoUI khi hiển thị win panel
        this.HideTowerInfoUI();
        
        // Ẩn EnemySpawnButton khi hiển thị win panel
        this.HideEnemySpawnButton();
        
        // Nếu WinPanel null, thử tìm lại trong scene hiện tại
        if (winPanel == null)
        {
            Debug.LogWarning("WinPanel is null, attempting to find it again in current scene...");
            this.winPanel = this.FindPanelInCurrentScene("WinPanel");
        }
        
        if (winPanel != null) 
        {
            Debug.Log("Win panel found, activating...");
            winPanel.SetActive(true);
            
        // Đảm bảo WinText được hiện khi WinPanel hiện
        this.EnsureWinTextVisible();
        
        // Dừng nhạc nền khi win
        this.StopBackgroundMusicOnWinLose();
        
        // Kiểm tra CanvasGroup trước khi gọi StartCoroutine
            if (winPanelCanvasGroup != null)
            {
                // Dừng coroutine cũ nếu có
                if (winPanelFadeCoroutine != null)
                {
                    StopCoroutine(winPanelFadeCoroutine);
                }
                
                winPanelFadeCoroutine = StartCoroutine(FadeInPanel(winPanelCanvasGroup));
                Debug.Log("Win panel activated successfully!");
            }
            else
            {
                Debug.LogWarning("WinPanelCanvasGroup is null! Cannot start fade in animation.");
            }
        }
        else
        {
            Debug.LogError("Win panel is NULL! Cannot show win panel!");
            Debug.LogError("Please check if WinPanel exists in scene Hai_Map or assign it in Map1GameResultManager Inspector!");
            
            // Tạo WinPanel tạm thời nếu không tìm thấy
            this.CreateTemporaryWinPanel();
        }
        
        if (losePanel != null) losePanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        
        // Hoàn thành map hiện tại
        this.CompleteCurrentMap();
        
        Debug.Log("=== MAP1 ShowWinPanel() completed ===");
    }
    
    protected virtual void ShowLosePanel()
    {
        Debug.Log("=== MAP1 ShowLosePanel() called ===");
        Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        
        isGameEnded = true;
        
        // Ẩn quest panel khi hiển thị lose panel
        this.HideQuestPanel();
        
        // Ẩn TowerInfoUI khi hiển thị lose panel
        this.HideTowerInfoUI();
        
        // Ẩn EnemySpawnButton khi hiển thị lose panel
        this.HideEnemySpawnButton();
        
        // Nếu LosePanel null, thử tìm lại trong scene hiện tại
        if (losePanel == null)
        {
            Debug.LogWarning("LosePanel is null, attempting to find it again in current scene...");
            this.losePanel = this.FindPanelInCurrentScene("LosePanel");
        }
        
        if (losePanel != null) 
        {
            Debug.Log("Lose panel found, activating...");
            losePanel.SetActive(true);
            
        // Đảm bảo LoseText được hiện khi LosePanel hiện
        this.EnsureLoseTextVisible();
        
        // Dừng nhạc nền khi lose
        this.StopBackgroundMusicOnWinLose();
        
        // Kiểm tra CanvasGroup trước khi gọi StartCoroutine
            if (losePanelCanvasGroup != null)
            {
                // Dừng coroutine cũ nếu có
                if (losePanelFadeCoroutine != null)
                {
                    StopCoroutine(losePanelFadeCoroutine);
                }
                
                losePanelFadeCoroutine = StartCoroutine(FadeInPanel(losePanelCanvasGroup));
                Debug.Log("Lose panel activated successfully!");
            }
            else
            {
                Debug.LogWarning("LosePanelCanvasGroup is null! Cannot start fade in animation.");
            }
        }
        else
        {
            Debug.LogError("Lose panel is NULL! Cannot show lose panel!");
            Debug.LogError("Please check if LosePanel exists in scene Hai_Map or assign it in Map1GameResultManager Inspector!");
            
            // Tạo LosePanel tạm thời nếu không tìm thấy
            this.CreateTemporaryLosePanel();
        }
        
        if (winPanel != null) winPanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        
        Debug.Log("=== MAP1 ShowLosePanel() completed ===");
    }
    
    /// <summary>
    /// Kiểm tra xem game đã kết thúc chưa (win hoặc lose)
    /// </summary>
    public virtual bool IsGameEnded()
    {
        return this.isGameEnded;
    }
    
    /// <summary>
    /// Hiện WinPanel khi player thắng
    /// </summary>
    public virtual void ShowWinPanelPublic()
    {
        try
        {
            Debug.Log("=== SHOWING WIN PANEL PUBLIC ===");
            this.ShowWinPanel();
            Debug.Log("Win panel shown successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ShowWinPanelPublic: {e.Message}");
        }
    }
    
    /// <summary>
    /// Hiện LosePanel khi player thua
    /// </summary>
    public virtual void ShowLosePanelPublic()
    {
        try
        {
            Debug.Log("=== SHOWING LOSE PANEL PUBLIC ===");
            this.ShowLosePanel();
            Debug.Log("Lose panel shown successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ShowLosePanelPublic: {e.Message}");
        }
    }
    
    /// <summary>
    /// Được gọi khi hoàn thành tất cả enemy waves
    /// </summary>
    public virtual void OnAllWavesCompleted()
    {
        try
        {
            Debug.Log("=== MAP1 ALL WAVES COMPLETED ===");
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            Debug.Log($"IsMap1(): {IsMap1()}");
            Debug.Log($"isGameEnded before: {isGameEnded}");
            Debug.Log($"isWin: {isWin}");
            
            // Chỉ hiển thị win panel cho map 1 (Hai_Map)
            if (IsMap1())
            {
                Debug.Log("✅ Map 1 detected - Processing win condition...");
                
                // Kiểm tra xem game đã kết thúc chưa
                if (isGameEnded)
                {
                    Debug.LogWarning("⚠️ Game already ended! Cannot process wave completion.");
                    return;
                }
                
                isWin = true;
                isGameEnded = true;
                
                // Tắt tất cả SFX ngay lập tức, giữ lại nhạc
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.VolumeSfxUpdating(0f);
                    Debug.Log("Map 1: Đã tắt tất cả SFX, giữ lại nhạc nền");
                }
                
                // Hiển thị win panel
                Debug.Log("About to call ShowWinPanel() for wave completion...");
                ShowWinPanel();
                
                Debug.Log("✅ All waves completed successfully - Win panel shown!");
            }
            else
            {
                Debug.Log("Not Map 1, skipping wave completion processing");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong Map1 OnAllWavesCompleted: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Ẩn tất cả Win/Lose panel trong scene khi start game
    /// </summary>
    protected virtual void HideAllWinLosePanelsInScene()
    {
        try
        {
            Debug.Log("=== HIDING ALL WIN/LOSE PANELS IN SCENE ===");
            
            // Tìm tất cả panel có thể có trong scene
            GameObject[] allPanels = FindObjectsOfType<GameObject>();
            int hiddenCount = 0;
            
            foreach (GameObject obj in allPanels)
            {
                if (obj != null && (obj.name.Contains("WinPanel") || obj.name.Contains("LosePanel")))
                {
                    if (obj.activeSelf)
                    {
                        obj.SetActive(false);
                        hiddenCount++;
                        Debug.Log($"Hidden panel: {obj.name}");
                    }
                }
                // Không ẩn WinText vì nó là component con của WinPanel
                // Chỉ ẩn WinPanel và LosePanel chính
            }
            
            Debug.Log($"Hidden {hiddenCount} panels in scene");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in HideAllWinLosePanelsInScene: {e.Message}");
        }
    }
    
    /// <summary>
    /// Tìm WinPanel và LosePanel trong scene hiện tại
    /// </summary>
    protected virtual void FindWinLosePanelsInCurrentScene()
    {
        try
        {
            Debug.Log("=== FINDING WIN/LOSE PANELS IN CURRENT SCENE ===");
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            
            // Tìm WinPanel
            this.winPanel = this.FindPanelInCurrentScene("WinPanel");
            if (this.winPanel != null)
            {
                Debug.Log($"WinPanel found at path: {this.winPanel.transform.GetPath()}");
                // Ẩn panel khi tìm thấy để đảm bảo không hiện khi start game
                this.winPanel.SetActive(false);
                Debug.Log("WinPanel hidden on start");
            }
            else
            {
                Debug.LogWarning("WinPanel not found in current scene");
            }
            
            // Tìm LosePanel
            this.losePanel = this.FindPanelInCurrentScene("LosePanel");
            if (this.losePanel != null)
            {
                Debug.Log($"LosePanel found at path: {this.losePanel.transform.GetPath()}");
                // Ẩn panel khi tìm thấy để đảm bảo không hiện khi start game
                this.losePanel.SetActive(false);
                Debug.Log("LosePanel hidden on start");
            }
            else
            {
                Debug.LogWarning("LosePanel not found in current scene");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in FindWinLosePanelsInCurrentScene: {e.Message}");
        }
    }
    
    /// <summary>
    /// Tìm panel theo tên trong scene hiện tại
    /// </summary>
    protected virtual GameObject FindPanelInCurrentScene(string panelName)
    {
        try
        {
            Debug.Log($"=== SEARCHING FOR {panelName} ===");
            
            // Tìm theo tên chính xác
            GameObject panel = GameObject.Find(panelName);
            if (panel != null)
            {
                Debug.Log($"✅ Found {panelName} by exact name: {panel.name}");
                return panel;
            }
            
            // Tìm theo tên chứa (case insensitive)
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj != null && obj.name.ToLower().Contains(panelName.ToLower()))
                {
                    Debug.Log($"✅ Found {panelName} by containing name: {obj.name}");
                    return obj;
                }
            }
            
            // Tìm theo tag nếu có (chỉ khi tag tồn tại)
            try
            {
                if (panelName == "WinPanel")
                {
                    GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("WinPanel");
                    if (taggedObjects.Length > 0)
                    {
                        Debug.Log($"✅ Found {panelName} by tag: {taggedObjects[0].name}");
                        return taggedObjects[0];
                    }
                }
                
                if (panelName == "LosePanel")
                {
                    GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("LosePanel");
                    if (taggedObjects.Length > 0)
                    {
                        Debug.Log($"✅ Found {panelName} by tag: {taggedObjects[0].name}");
                        return taggedObjects[0];
                    }
                }
            }
            catch (System.Exception tagException)
            {
                Debug.LogWarning($"Tag search failed (tags not defined): {tagException.Message}");
                // Tiếp tục tìm bằng cách khác
            }
            
            // Tìm trong Canvas hierarchy
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                // Tìm trực tiếp trong Canvas
                Transform panelTransform = canvas.transform.Find(panelName);
                if (panelTransform != null)
                {
                    Debug.Log($"✅ Found {panelName} in Canvas: {panelTransform.name}");
                    return panelTransform.gameObject;
                }
                
                // Tìm trong UI hierarchy (Canvas/UI/Panel)
                Transform uiTransform = canvas.transform.Find("UI");
                if (uiTransform != null)
                {
                    panelTransform = uiTransform.Find(panelName);
                    if (panelTransform != null)
                    {
                        Debug.Log($"✅ Found {panelName} in UI hierarchy: {panelTransform.name}");
                        return panelTransform.gameObject;
                    }
                }
                
                // Tìm trong tất cả child của Canvas
                panelTransform = FindInChildren(canvas.transform, panelName);
                if (panelTransform != null)
                {
                    Debug.Log($"✅ Found {panelName} in Canvas children: {panelTransform.name}");
                    return panelTransform.gameObject;
                }
            }
            
            Debug.LogWarning($"❌ Panel {panelName} not found in scene");
            Debug.Log($"Total objects in scene: {allObjects.Length}");
            Debug.Log($"Total canvases in scene: {canvases.Length}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in FindPanelInCurrentScene: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Tìm object trong tất cả children của parent
    /// </summary>
    protected virtual Transform FindInChildren(Transform parent, string objectName)
    {
        try
        {
            // Kiểm tra chính parent
            if (parent.name == objectName)
            {
                return parent;
            }
            
            // Tìm trong tất cả children
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Transform found = FindInChildren(child, objectName);
                if (found != null)
                {
                    return found;
                }
            }
            
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in FindInChildren: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Khởi tạo CanvasGroup cho Win/Lose panel
    /// </summary>
    protected virtual void InitializeCanvasGroups()
    {
        try
        {
            Debug.Log("=== INITIALIZING CANVAS GROUPS ===");
            
            // Khởi tạo WinPanelCanvasGroup
            if (this.winPanel != null)
            {
                this.winPanelCanvasGroup = this.winPanel.GetComponent<CanvasGroup>();
                if (this.winPanelCanvasGroup == null)
                {
                    this.winPanelCanvasGroup = this.winPanel.AddComponent<CanvasGroup>();
                    Debug.Log("Added CanvasGroup to WinPanel");
                }
                else
                {
                    Debug.Log("WinPanelCanvasGroup found");
                }
            }
            else
            {
                Debug.LogWarning("WinPanel is null, cannot initialize CanvasGroup");
            }
            
            // Khởi tạo LosePanelCanvasGroup
            if (this.losePanel != null)
            {
                this.losePanelCanvasGroup = this.losePanel.GetComponent<CanvasGroup>();
                if (this.losePanelCanvasGroup == null)
                {
                    this.losePanelCanvasGroup = this.losePanel.AddComponent<CanvasGroup>();
                    Debug.Log("Added CanvasGroup to LosePanel");
                }
                else
                {
                    Debug.Log("LosePanelCanvasGroup found");
                }
            }
            else
            {
                Debug.LogWarning("LosePanel is null, cannot initialize CanvasGroup");
            }
            
            Debug.Log("Canvas groups initialization completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in InitializeCanvasGroups: {e.Message}");
        }
    }
    
    /// <summary>
    /// Dừng tất cả coroutine fade
    /// </summary>
    protected virtual void StopAllFadeCoroutines()
    {
        try
        {
            if (this.winPanelFadeCoroutine != null)
            {
                StopCoroutine(this.winPanelFadeCoroutine);
                this.winPanelFadeCoroutine = null;
                Debug.Log("WinPanel fade coroutine stopped");
            }
            
            if (this.losePanelFadeCoroutine != null)
            {
                StopCoroutine(this.losePanelFadeCoroutine);
                this.losePanelFadeCoroutine = null;
                Debug.Log("LosePanel fade coroutine stopped");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopAllFadeCoroutines: {e.Message}");
        }
    }
    
    /// <summary>
    /// Hiện TowerInfoUI
    /// </summary>
    protected virtual void ShowTowerInfoUI()
    {
        try
        {
            // Tìm TowerInfoUI trong scene
            GameObject towerInfoUI = GameObject.Find("TowerInfoUI");
            if (towerInfoUI != null)
            {
                towerInfoUI.SetActive(true);
                Debug.Log("TowerInfoUI shown");
            }
            else
            {
                Debug.LogWarning("TowerInfoUI not found in scene");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ShowTowerInfoUI: {e.Message}");
        }
    }
    /// <summary>
    /// Fade in panel với hiệu ứng mượt mà
    /// </summary>
    protected virtual System.Collections.IEnumerator FadeInPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is null, cannot fade in panel");
            yield break;
        }
        
        Debug.Log("Starting fade in animation...");
        
        try
        {
            // Set alpha = 0 và hiện panel
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting up fade in: {e.Message}");
            yield break;
        }
        
        // Fade in từ 0 đến 1
        float fadeTime = 1f; // 1 giây
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeTime)
        {
            try
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error during fade animation: {e.Message}");
                yield break;
            }
            
            yield return null;
        }
        
        try
        {
            // Đảm bảo alpha = 1
            canvasGroup.alpha = 1f;
            Debug.Log("Fade in animation completed!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error completing fade animation: {e.Message}");
        }
    }
    
    protected virtual void HideQuestPanel() { }
    protected virtual void HideTowerInfoUI() { }
    protected virtual void HideEnemySpawnButton() { }
    protected virtual void CreateTemporaryWinPanel() { }
    protected virtual void CreateTemporaryLosePanel() { }
    protected virtual void CompleteCurrentMap() { }
    /// <summary>
    /// Ẩn QuestPanel từ SampleScene nếu có (do DontDestroyOnLoad)
    /// </summary>
    protected virtual void HideQuestPanelFromSampleScene()
    {
        try
        {
            Debug.Log("=== HIDING QUEST PANEL FROM SAMPLESCENE ===");
            
            // Ẩn TowerQuestUI nếu có
            if (TowerQuestUI.Instance != null)
            {
                TowerQuestUI.Instance.HideQuestPanel();
                Debug.Log("TowerQuestUI hidden for Map1");
            }
            
            // Tìm và ẩn QuestPanel trực tiếp trong scene
            GameObject questPanel = GameObject.Find("QuestPanel");
            if (questPanel != null)
            {
                questPanel.SetActive(false);
                Debug.Log("QuestPanel found and hidden for Map1");
            }
            else
            {
                Debug.Log("QuestPanel not found in Map1 scene");
            }
            
            // Tìm và ẩn NotificationPanel nếu có
            GameObject notificationPanel = GameObject.Find("NotificationPanel");
            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
                Debug.Log("NotificationPanel found and hidden for Map1");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error hiding QuestPanel from SampleScene: {e.Message}");
        }
    }
    
    /// <summary>
    /// Đảm bảo WinText được hiện khi WinPanel hiện
    /// </summary>
    protected virtual void EnsureWinTextVisible()
    {
        try
        {
            Debug.Log("=== ENSURING WIN TEXT VISIBLE ===");
            
            if (this.winPanel == null)
            {
                Debug.LogWarning("WinPanel is null, cannot ensure WinText visibility");
                return;
            }
            
            // Tìm WinText trong WinPanel
            Transform winTextTransform = this.winPanel.transform.Find("WinText");
            if (winTextTransform != null)
            {
                winTextTransform.gameObject.SetActive(true);
                Debug.Log("WinText found and activated");
            }
            else
            {
                Debug.LogWarning("WinText not found in WinPanel");
                
                // Tìm tất cả TextMeshProUGUI trong WinPanel
                TMPro.TextMeshProUGUI[] textComponents = this.winPanel.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
                Debug.Log($"Found {textComponents.Length} TextMeshProUGUI components in WinPanel");
                
                foreach (TMPro.TextMeshProUGUI textComponent in textComponents)
                {
                    if (textComponent != null)
                    {
                        textComponent.gameObject.SetActive(true);
                        Debug.Log($"Activated text component: {textComponent.name}");
                    }
                }
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error ensuring WinText visible: {e.Message}");
        }
    }
    
    /// <summary>
    /// Đảm bảo LoseText được hiện khi LosePanel hiện
    /// </summary>
    protected virtual void EnsureLoseTextVisible()
    {
        try
        {
            Debug.Log("=== ENSURING LOSE TEXT VISIBLE ===");
            
            if (this.losePanel == null)
            {
                Debug.LogWarning("LosePanel is null, cannot ensure LoseText visibility");
                return;
            }
            
            // Tìm LoseText trong LosePanel
            Transform loseTextTransform = this.losePanel.transform.Find("LoseText");
            if (loseTextTransform != null)
            {
                loseTextTransform.gameObject.SetActive(true);
                Debug.Log("LoseText found and activated");
            }
            else
            {
                Debug.LogWarning("LoseText not found in LosePanel");
                
                // Tìm tất cả TextMeshProUGUI trong LosePanel
                TMPro.TextMeshProUGUI[] textComponents = this.losePanel.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
                Debug.Log($"Found {textComponents.Length} TextMeshProUGUI components in LosePanel");
                
                foreach (TMPro.TextMeshProUGUI textComponent in textComponents)
                {
                    if (textComponent != null)
                    {
                        textComponent.gameObject.SetActive(true);
                        Debug.Log($"Activated text component: {textComponent.name}");
                    }
                }
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error ensuring LoseText visible: {e.Message}");
        }
    }
    
    /// <summary>
    /// Dừng nhạc nền khi Win/Lose
    /// </summary>
    protected virtual void StopBackgroundMusicOnWinLose()
    {
        try
        {
            Debug.Log("=== STOPPING BACKGROUND MUSIC ON WIN/LOSE ===");
            
            if (SoundManager.Instance != null)
            {
                // Tắt background music chính
                if (SoundManager.Instance.GetBackgroundMusic() != null)
                {
                    SoundManager.Instance.GetBackgroundMusic().gameObject.SetActive(false);
                    Debug.Log("Main background music stopped on Win/Lose");
                }
                
                // Tắt tất cả music trong listMusic
                if (SoundManager.Instance.GetSoundSpawnerCtrl() != null && 
                    SoundManager.Instance.GetSoundSpawnerCtrl().Spawner != null)
                {
                    // Tìm tất cả MusicCtrl trong scene và tắt chúng
                    MusicCtrl[] allMusic = FindObjectsOfType<MusicCtrl>();
                    int stoppedCount = 0;
                    
                    foreach (MusicCtrl music in allMusic)
                    {
                        if (music != null && music.gameObject.activeSelf)
                        {
                            music.gameObject.SetActive(false);
                            stoppedCount++;
                        }
                    }
                    
                    Debug.Log($"Stopped {stoppedCount} music objects on Win/Lose");
                }
                
                Debug.Log("Background music stopped successfully on Win/Lose!");
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot stop background music.");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error stopping background music on Win/Lose: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    protected virtual void EnsureSoundSystemInMap1() { }
}
