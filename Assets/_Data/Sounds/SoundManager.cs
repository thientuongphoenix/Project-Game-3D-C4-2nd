using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SaiSingleton<SoundManager>
{
    [SerializeField] protected SoundName bgName = SoundName.Narco;
    [SerializeField] protected MusicCtrl bgMusic;
    [SerializeField] protected SoundSpawnerCtrl ctrl;

    [Range(0f, 1f)]
    [SerializeField] protected float volumeMusic = 1f;

    [Range(0f, 1f)]
    [SerializeField] protected float volumeSfx = 1f;

    [SerializeField] protected List<MusicCtrl> listMusic;
    [SerializeField] protected List<SFXCtrl> listSfx;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        this.LoadSettings();
        this.StartMusicBackground();
        
        // Đảm bảo sound system được khởi tạo đầy đủ khi vào scene mới
        this.EnsureSoundSystemInitialized();
    }

    protected virtual void FixedUpdate()
    {
        // this.VolumeMusicUpdating();
        // this.VolumeSfxUpdating();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadSoundSpawnerCtrl();
        this.EnsureSoundSpawnerExists();
    }

    protected virtual void LoadSoundSpawnerCtrl()
    {
        if (this.ctrl != null) return;
        this.ctrl = GameObject.FindAnyObjectByType<SoundSpawnerCtrl>();
        
        if (this.ctrl == null)
        {
            Debug.LogWarning("SoundManager: Không tìm thấy SoundSpawnerCtrl trong scene! " +
                           "Sẽ tự động tạo SoundSpawnerCtrl...");
        }
        else
        {
            Debug.Log(transform.name + ": LoadSoundSpawnerCtrl thành công", gameObject);
        }
    }

    public virtual void StartMusicBackground()
    {
        // Debug: Hiển thị giá trị bgName hiện tại
        Debug.Log($"SoundManager: StartMusicBackground called with bgName = {this.bgName}");
        
        // Tắt tất cả nhạc nền cũ trước khi khởi động nhạc nền mới
        this.StopAllBackgroundMusic();
        
        // Kiểm tra xem background music đã tồn tại và đang active chưa
        if (this.bgMusic != null && this.bgMusic.gameObject.activeSelf)
        {
            Debug.Log("SoundManager: Background music is already active, skipping...");
            return;
        }
        
        if (this.bgMusic == null) 
        {
            Debug.Log($"SoundManager: Creating background music with soundName: {this.bgName}");
            this.bgMusic = this.CreateMusic(this.bgName);
            if (this.bgMusic == null)
            {
                Debug.LogError("SoundManager: Failed to create background music");
                return;
            }
        }
        
        // Đảm bảo background music được thêm vào listMusic để có thể cập nhật volume
        this.AddMusic(this.bgMusic);
        
        // Áp dụng volume hiện tại cho background music
        if (this.bgMusic.AudioSource != null)
        {
            this.bgMusic.AudioSource.volume = this.volumeMusic;
        }
        
        this.bgMusic.gameObject.SetActive(true);
        Debug.Log("SoundManager: Background music started and added to volume control list");
    }

    protected virtual MusicCtrl CreateBackgroundMusic()
    {
        MusicCtrl musicPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName(this.bgName.ToString());
        return (MusicCtrl)this.ctrl.Spawner.Spawn(musicPrefab, Vector3.zero);
        //Phương thức GetByName() có thể trả về một đối tượng có kiểu dữ liệu cơ bản (base type), nhưng ta cần sử dụng các tính năng đặc thù của MusicCtrl. Việc ép kiểu giúp ta có thể truy cập các phương thức và thuộc tính riêng của MusicCtrl.
        // Tương tự, phương thức Spawn() có thể trả về một đối tượng có kiểu dữ liệu cơ bản, nhưng ta cần đảm bảo rằng đối tượng được tạo ra là một MusicCtrl để có thể sử dụng các chức năng đặc thù của nó.
    }

    public virtual void ToggleMusic()
    {
        if (this.bgMusic == null)
        {
            this.StartMusicBackground();
            return;
        }

        bool status = this.bgMusic.gameObject.activeSelf;
        this.bgMusic.gameObject.SetActive(!status);
        
        // Đảm bảo background music được thêm vào listMusic nếu chưa có
        this.AddMusic(this.bgMusic);
        
        // Cập nhật volume khi toggle
        if (this.bgMusic.AudioSource != null)
        {
            this.bgMusic.AudioSource.volume = this.volumeMusic;
        }
        
        Debug.Log($"SoundManager: Music toggled to {(status ? "OFF" : "ON")} with volume {this.volumeMusic}");
    }

    public virtual MusicCtrl CreateMusic(SoundName soundName)
    {
        Debug.Log($"SoundManager: CreateMusic called with soundName = {soundName}");
        
        if (this.ctrl == null)
        {
            Debug.LogError("SoundManager: ctrl is null, trying to reload...");
            this.LoadSoundSpawnerCtrl();
            if (this.ctrl == null)
            {
                Debug.LogError("SoundManager: Failed to load SoundSpawnerCtrl");
                return null;
            }
        }
        
        // Kiểm tra Prefabs có null không
        if (this.ctrl.Prefabs == null)
        {
            Debug.LogError("SoundManager: ctrl.Prefabs is null! Trying to fix...");
            this.CheckAndFixSoundSpawnerCtrl();
            if (this.ctrl.Prefabs == null)
            {
                Debug.LogError("SoundManager: Failed to fix Prefabs");
                return null;
            }
        }
        
        MusicCtrl soundPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName(soundName.ToString());
        if (soundPrefab == null)
        {
            Debug.LogError($"SoundManager: Không tìm thấy prefab cho soundName: {soundName}");
            
            // Thử tìm prefab khác làm fallback
            if (soundName == SoundName.BackgroundMusic || soundName == SoundName.MyNewMusic)
            {
                Debug.LogWarning("SoundManager: Thử tìm prefab khác làm fallback...");
                soundPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName("Narco");
                if (soundPrefab == null)
                {
                    soundPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName("NewBackgroundMusic");
                }
                if (soundPrefab == null)
                {
                    soundPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName("Menu");
                }
                if (soundPrefab == null)
                {
                    soundPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName("Tutorial");
                }
                
                if (soundPrefab != null)
                {
                    Debug.LogWarning($"SoundManager: Sử dụng prefab fallback: {soundPrefab.name}");
                }
                else
                {
                    Debug.LogError("SoundManager: Không tìm thấy prefab nào để làm fallback!");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        
        return this.CreateMusic(soundPrefab);
    }

    public virtual MusicCtrl CreateMusic(MusicCtrl musicPrefab)
    {
        if (this.ctrl == null)
        {
            Debug.LogError("SoundManager: ctrl is null in CreateMusic(MusicCtrl)");
            return null;
        }
        
        if (musicPrefab == null)
        {
            Debug.LogError("SoundManager: musicPrefab is null");
            return null;
        }
        
        MusicCtrl newMusic = (MusicCtrl)this.ctrl.Spawner.Spawn(musicPrefab, Vector3.zero);
        if (newMusic != null && newMusic.AudioSource != null)
        {
            newMusic.AudioSource.volume = this.volumeMusic;
            this.AddMusic(newMusic);
        }
        return newMusic;
    }

    public virtual void AddMusic(MusicCtrl newMusic)
    {
        // Khởi tạo listMusic nếu null
        if (this.listMusic == null)
        {
            this.listMusic = new System.Collections.Generic.List<MusicCtrl>();
        }
        
        if (newMusic == null) return;
        
        if(this.listMusic.Contains(newMusic)) return;
        this.listMusic.Add(newMusic);
    }

    public virtual SFXCtrl CreateSfx(SoundName soundName)
    {
        if (this.ctrl == null)
        {
            Debug.LogError("SoundManager: ctrl is null, trying to reload...");
            this.LoadSoundSpawnerCtrl();
            if (this.ctrl == null)
            {
                Debug.LogError("SoundManager: Failed to load SoundSpawnerCtrl");
                return null;
            }
        }
        
        // Kiểm tra Prefabs có null không
        if (this.ctrl.Prefabs == null)
        {
            Debug.LogError("SoundManager: ctrl.Prefabs is null! Trying to fix...");
            this.CheckAndFixSoundSpawnerCtrl();
            if (this.ctrl.Prefabs == null)
            {
                Debug.LogError("SoundManager: Failed to fix Prefabs");
                return null;
            }
        }
        
        SFXCtrl soundPrefab = (SFXCtrl)this.ctrl.Prefabs.GetByName(soundName.ToString());
        if (soundPrefab == null)
        {
            Debug.LogError($"SoundManager: Không tìm thấy prefab cho soundName: {soundName}");
            return null;
        }
        
        return this.CreateSfx(soundPrefab);
    }

    public virtual SFXCtrl CreateSfx(SFXCtrl sfxPrefab)
    {
        if (this.ctrl == null)
        {
            Debug.LogError("SoundManager: ctrl is null in CreateSfx(SFXCtrl)");
            return null;
        }
        
        if (sfxPrefab == null)
        {
            Debug.LogError("SoundManager: sfxPrefab is null");
            return null;
        }
        
        SFXCtrl newSound = (SFXCtrl)this.ctrl.Spawner.Spawn(sfxPrefab, Vector3.zero);
        //Điều này đảm bảo rằng sfx mới cũng được cập nhật giá trị volume, vì set volume chỉ ảnh hưởng tới sfx đã sinh ra rồi và được add vào listSfx.
        if (newSound != null && newSound.AudioSource != null)
        {
            newSound.AudioSource.volume = this.volumeSfx;
            this.AddSfx(newSound);
            
            // Activate và play SFX
            newSound.gameObject.SetActive(true);
            newSound.AudioSource.Play();
        }
        return newSound;
    }

    public virtual void AddSfx(SFXCtrl newSound)
    {
        if (this.listSfx == null)
        {
            this.listSfx = new System.Collections.Generic.List<SFXCtrl>();
        }
        
        if (newSound == null) return;
        
        if (this.listSfx.Contains(newSound)) return;
        this.listSfx.Add(newSound);
    }

    public virtual void VolumeMusicUpdating(float volume)
    {
        this.volumeMusic = volume;
        
        // Cập nhật volume cho tất cả music trong list
        int updatedCount = 0;
        foreach(MusicCtrl musicCtrl in this.listMusic)
        {
            if (musicCtrl != null && musicCtrl.AudioSource != null)
            {
                musicCtrl.AudioSource.volume = this.volumeMusic;
                updatedCount++;
            }
        }
        
        // Đảm bảo background music cũng được cập nhật volume
        this.UpdateBackgroundMusicVolume();
        
        
        // Tìm và cập nhật tất cả Music trong scene (fallback)
        this.UpdateAllMusicInScene();
        
        // Lưu settings khi volume thay đổi
        this.SaveSettings();
        
        Debug.Log($"SoundManager: Music volume updated to {this.volumeMusic} (Updated {updatedCount} Music in list)");
    }

    public virtual void VolumeSfxUpdating(float volume)
    {
        this.volumeSfx = volume;
        
        // Khởi tạo listSfx nếu null
        if (this.listSfx == null)
        {
            Debug.LogWarning("SoundManager: listSfx is null, initializing...");
            this.listSfx = new System.Collections.Generic.List<SFXCtrl>();
        }
        
        // Cập nhật volume cho tất cả SFX trong list
        int updatedCount = 0;
        foreach(SFXCtrl sfxCtrl in this.listSfx)
        {
            if (sfxCtrl != null && sfxCtrl.AudioSource != null)
            {
                sfxCtrl.AudioSource.volume = this.volumeSfx;
                updatedCount++;
            }
        }
        
        // Tìm và cập nhật tất cả SFX trong scene (fallback)
        this.UpdateAllSFXInScene();
        
        // Lưu settings khi volume thay đổi
        this.SaveSettings();
        
        Debug.Log($"SoundManager: SFX volume updated to {this.volumeSfx} (Updated {updatedCount} SFX in list)");
    }
    
    /// <summary>
    /// Lưu music và SFX settings vào PlayerPrefs
    /// </summary>
    protected virtual void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", this.volumeMusic);
        PlayerPrefs.SetFloat("SFXVolume", this.volumeSfx);
        PlayerPrefs.Save();
        Debug.Log($"SoundManager: Settings saved - Music: {this.volumeMusic}, SFX: {this.volumeSfx}");
    }
    
    /// <summary>
    /// Load music và SFX settings từ PlayerPrefs
    /// </summary>
    protected virtual void LoadSettings()
    {
        // Load music volume (default = 1f)
        this.volumeMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        // Load SFX volume (default = 1f)
        this.volumeSfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        Debug.Log($"SoundManager: Settings loaded - Music: {this.volumeMusic}, SFX: {this.volumeSfx}");
        
        // Áp dụng settings ngay lập tức
        this.ApplyLoadedSettings();
    }
    
    /// <summary>
    /// Áp dụng settings đã load cho tất cả music và SFX hiện có
    /// </summary>
    protected virtual void ApplyLoadedSettings()
    {
        // Đảm bảo background music được thêm vào list
        this.EnsureBackgroundMusicInList();
        
        // Áp dụng cho music trong list
        if (this.listMusic != null)
        {
            foreach(MusicCtrl musicCtrl in this.listMusic)
            {
                if (musicCtrl != null && musicCtrl.AudioSource != null)
                {
                    musicCtrl.AudioSource.volume = this.volumeMusic;
                }
            }
        }
        
        // Áp dụng cho SFX trong list
        if (this.listSfx != null)
        {
            foreach(SFXCtrl sfxCtrl in this.listSfx)
            {
                if (sfxCtrl != null && sfxCtrl.AudioSource != null)
                {
                    sfxCtrl.AudioSource.volume = this.volumeSfx;
                }
            }
        }
        
        // Tìm và cập nhật tất cả Music và SFX trong scene (fallback)
        this.UpdateAllMusicInScene();
        this.UpdateAllSFXInScene();
        
        // Đảm bảo background music được cập nhật volume
        this.UpdateBackgroundMusicVolume();
        
        Debug.Log("SoundManager: Loaded settings applied to all music and SFX");
    }

    /// <summary>
    /// Đảm bảo SoundSpawnerCtrl tồn tại trong scene
    /// </summary>
    protected virtual void EnsureSoundSpawnerExists()
    {
        if (this.ctrl != null) return;
        
        // Tìm lại SoundSpawnerCtrl
        this.ctrl = GameObject.FindAnyObjectByType<SoundSpawnerCtrl>();
        
        if (this.ctrl == null)
        {
            Debug.LogWarning("SoundManager: Tự động tạo SoundSpawnerCtrl...");
            this.CreateSoundSpawnerCtrl();
            
            // Kiểm tra lại sau khi tạo
            if (this.ctrl != null)
            {
                Debug.Log("SoundManager: SoundSpawnerCtrl đã được tạo thành công!");
            }
            else
            {
                Debug.LogError("SoundManager: Không thể tạo SoundSpawnerCtrl!");
            }
        }
        else
        {
            Debug.Log("SoundManager: SoundSpawnerCtrl đã tồn tại trong scene");
        }
    }

    /// <summary>
    /// Tạo SoundSpawnerCtrl nếu không có
    /// </summary>
    protected virtual void CreateSoundSpawnerCtrl()
    {
        try
        {
            // Tạo GameObject chính
            GameObject soundSpawnerObj = new GameObject("SoundSpawnerCtrl");
            
            // Thêm SoundSpawnerCtrl component
            this.ctrl = soundSpawnerObj.AddComponent<SoundSpawnerCtrl>();
            
            // Thêm SoundSpawner component
            SoundSpawner spawner = soundSpawnerObj.AddComponent<SoundSpawner>();
            
            // Tạo SoundPrefabs GameObject
            GameObject soundPrefabsObj = new GameObject("SoundPrefabs");
            soundPrefabsObj.transform.SetParent(soundSpawnerObj.transform);
            
            // Thêm SoundPrefabs component
            soundPrefabsObj.AddComponent<SoundPrefabs>();
            
            // Tạo PoolHolder
            GameObject poolHolderObj = new GameObject("PoolHolder");
            poolHolderObj.transform.SetParent(soundSpawnerObj.transform);
            
            // Cấu hình SoundSpawner (poolHolder sẽ được set tự động)
            // spawner.poolHolder = poolHolderObj.transform;
            
            // Load components để cập nhật reference
            this.ctrl.LoadSoundPrefabsPublic();
            this.ctrl.LoadSoundSpawnerPublic();
            
            // Đảm bảo không bị destroy khi load scene mới (chỉ trong Play mode)
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(soundSpawnerObj);
            }
            
            Debug.Log("SoundManager: SoundSpawnerCtrl đã được tạo tự động với cấu trúc đầy đủ");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SoundManager: Lỗi khi tạo SoundSpawnerCtrl: {e.Message}");
            this.ctrl = null;
        }
    }
    
    /// <summary>
    /// Kiểm tra và sửa lỗi SoundSpawnerCtrl nếu cần
    /// </summary>
    [ContextMenu("Check and Fix SoundSpawnerCtrl")]
    public virtual void CheckAndFixSoundSpawnerCtrl()
    {
        Debug.Log("=== KIỂM TRA SOUND SPAWNER CTRL ===");
        
        // Kiểm tra SoundSpawnerCtrl
        if (this.ctrl == null)
        {
            Debug.LogWarning("❌ SoundSpawnerCtrl is null! Đang tìm lại...");
            this.ctrl = GameObject.FindAnyObjectByType<SoundSpawnerCtrl>();
        }
        
        if (this.ctrl == null)
        {
            Debug.LogWarning("❌ Không tìm thấy SoundSpawnerCtrl! Đang tạo mới...");
            this.CreateSoundSpawnerCtrl();
        }
        
        if (this.ctrl != null)
        {
            Debug.Log("✅ SoundSpawnerCtrl: OK");
            
            // Kiểm tra SoundSpawner
            if (this.ctrl.Spawner == null)
            {
                Debug.LogWarning("⚠️ SoundSpawner is null! Đang thêm component...");
                this.ctrl.gameObject.AddComponent<SoundSpawner>();
            }
            else
            {
                Debug.Log("✅ SoundSpawner: OK");
            }
            
            // Kiểm tra SoundPrefabs
            if (this.ctrl.Prefabs == null)
            {
                Debug.LogWarning("⚠️ SoundPrefabs is null! Đang tạo...");
                GameObject soundPrefabsObj = new GameObject("SoundPrefabs");
                soundPrefabsObj.transform.SetParent(this.ctrl.transform);
                soundPrefabsObj.AddComponent<SoundPrefabs>();
                
                // Gọi LoadSoundPrefabs để cập nhật reference
                this.ctrl.LoadSoundPrefabsPublic();
                Debug.Log("✅ SoundPrefabs đã được tạo và load reference");
            }
            else
            {
                Debug.Log("✅ SoundPrefabs: OK");
            }
        }
        else
        {
            Debug.LogError("❌ Không thể tạo SoundSpawnerCtrl!");
        }
        
        Debug.Log("=== KIỂM TRA HOÀN TẤT ===");
    }
    
    /// <summary>
    /// Debug method để kiểm tra tất cả prefabs có sẵn
    /// </summary>
    [ContextMenu("Debug Available Prefabs")]
    public virtual void DebugAvailablePrefabs()
    {
        Debug.Log("=== DEBUG AVAILABLE PREFABS ===");
        
        if (this.ctrl == null)
        {
            Debug.LogError("SoundSpawnerCtrl is null!");
            return;
        }
        
        if (this.ctrl.Prefabs == null)
        {
            Debug.LogError("SoundPrefabs is null!");
            return;
        }
        
        Debug.Log($"Total prefabs found: {this.ctrl.Prefabs.PrefabsList.Count}");
        
        foreach (var prefab in this.ctrl.Prefabs.PrefabsList)
        {
            if (prefab != null)
            {
                Debug.Log($"- Prefab: {prefab.name} (Type: {prefab.GetType().Name})");
            }
            else
            {
                Debug.LogWarning("- Prefab: NULL");
            }
        }
        
        Debug.Log("=== DEBUG COMPLETED ===");
    }
    
    /// <summary>
    /// Get current music volume (for UI synchronization)
    /// </summary>
    public virtual float GetMusicVolume()
    {
        return this.volumeMusic;
    }
    
    /// <summary>
    /// Get current SFX volume (for UI synchronization)
    /// </summary>
    public virtual float GetSFXVolume()
    {
        return this.volumeSfx;
    }
    
    /// <summary>
    /// Get SoundSpawnerCtrl (for external access)
    /// </summary>
    public virtual SoundSpawnerCtrl GetSoundSpawnerCtrl()
    {
        return this.ctrl;
    }
    
    /// <summary>
    /// Check if SoundSpawnerCtrl is null
    /// </summary>
    public virtual bool IsSoundSpawnerCtrlNull()
    {
        return this.ctrl == null;
    }
    
    /// <summary>
    /// Get background music (for external access)
    /// </summary>
    public virtual MusicCtrl GetBackgroundMusic()
    {
        return this.bgMusic;
    }
    
    /// <summary>
    /// Check if background music is null or inactive
    /// </summary>
    public virtual bool IsBackgroundMusicNullOrInactive()
    {
        return this.bgMusic == null || !this.bgMusic.gameObject.activeSelf;
    }
    
    /// <summary>
    /// Public method to load SoundSpawnerCtrl (for external access)
    /// </summary>
    public virtual void LoadSoundSpawnerCtrlPublic()
    {
        this.LoadSoundSpawnerCtrl();
    }
    
    /// <summary>
    /// Public method to ensure SoundSpawner exists (for external access)
    /// </summary>
    public virtual void EnsureSoundSpawnerExistsPublic()
    {
        this.EnsureSoundSpawnerExists();
    }
    
    /// <summary>
    /// Public method to load settings (for external access)
    /// </summary>
    public virtual void LoadSettingsPublic()
    {
        this.LoadSettings();
    }
    
    /// <summary>
    /// Cập nhật volume cho background music
    /// </summary>
    protected virtual void UpdateBackgroundMusicVolume()
    {
        if (this.bgMusic != null && this.bgMusic.AudioSource != null)
        {
            this.bgMusic.AudioSource.volume = this.volumeMusic;
            Debug.Log($"SoundManager: Background music volume updated to {this.volumeMusic}");
        }
    }
    
    
    /// <summary>
    /// Đảm bảo background music được thêm vào listMusic để quản lý volume
    /// </summary>
    protected virtual void EnsureBackgroundMusicInList()
    {
        if (this.bgMusic != null)
        {
            this.AddMusic(this.bgMusic);
        }
    }
    
    /// <summary>
    /// Tìm và cập nhật tất cả SFX trong scene (fallback method)
    /// </summary>
    protected virtual void UpdateAllSFXInScene()
    {
        // Tìm tất cả SFXCtrl trong scene
        SFXCtrl[] allSFX = FindObjectsOfType<SFXCtrl>();
        int sceneUpdatedCount = 0;
        
        foreach (SFXCtrl sfx in allSFX)
        {
            if (sfx != null && sfx.AudioSource != null)
            {
                sfx.AudioSource.volume = this.volumeSfx;
                sceneUpdatedCount++;
                
                // Thêm vào list nếu chưa có
                this.AddSfx(sfx);
            }
        }
        
        if (sceneUpdatedCount > 0)
        {
            Debug.Log($"SoundManager: Updated {sceneUpdatedCount} SFX found in scene");
        }
    }
    
    /// <summary>
    /// Tìm và cập nhật tất cả Music trong scene (fallback method)
    /// </summary>
    protected virtual void UpdateAllMusicInScene()
    {
        // Tìm tất cả MusicCtrl trong scene
        MusicCtrl[] allMusic = FindObjectsOfType<MusicCtrl>();
        int sceneUpdatedCount = 0;
        
        foreach (MusicCtrl music in allMusic)
        {
            if (music != null && music.AudioSource != null)
            {
                music.AudioSource.volume = this.volumeMusic;
                sceneUpdatedCount++;
                
                // Thêm vào list nếu chưa có
                this.AddMusic(music);
            }
        }
        
        if (sceneUpdatedCount > 0)
        {
            Debug.Log($"SoundManager: Updated {sceneUpdatedCount} Music found in scene");
        }
    }
    
    /// <summary>
    /// Tắt background music khi đang ở cutscene
    /// </summary>
    public virtual void StopBackgroundMusicForCutscene()
    {
        try
        {
            Debug.Log("=== STOPPING BACKGROUND MUSIC FOR CUTSCENE ===");
            
            if (this.bgMusic != null)
            {
                this.bgMusic.gameObject.SetActive(false);
                Debug.Log("Background music stopped for cutscene!");
            }
            else
            {
                Debug.Log("Background music is already stopped or null");
            }
            
            Debug.Log("Background music stop for cutscene completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopBackgroundMusicForCutscene: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Khởi động lại background music sau khi kết thúc cutscene
    /// </summary>
    public virtual void ResumeBackgroundMusicAfterCutscene()
    {
        try
        {
            Debug.Log("=== RESUMING BACKGROUND MUSIC AFTER CUTSCENE ===");
            
            if (this.bgMusic != null)
            {
                this.bgMusic.gameObject.SetActive(true);
                Debug.Log("Background music resumed after cutscene!");
            }
            else
            {
                Debug.Log("Background music is null, starting new one...");
                this.StartMusicBackground();
            }
            
            Debug.Log("Background music resume after cutscene completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ResumeBackgroundMusicAfterCutscene: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Tắt tất cả nhạc nền hiện có
    /// </summary>
    protected virtual void StopAllBackgroundMusic()
    {
        try
        {
            Debug.Log("=== STOPPING ALL BACKGROUND MUSIC ===");
            
            // Tắt background music chính
            if (this.bgMusic != null)
            {
                this.bgMusic.gameObject.SetActive(false);
                Debug.Log("Main background music stopped");
            }
            
            // Tắt tất cả music trong listMusic
            if (this.listMusic != null)
            {
                int stoppedCount = 0;
                foreach (MusicCtrl music in this.listMusic)
                {
                    if (music != null && music.gameObject.activeSelf)
                    {
                        music.gameObject.SetActive(false);
                        stoppedCount++;
                    }
                }
                Debug.Log($"Stopped {stoppedCount} music in listMusic");
            }
            
            // Tắt tất cả MusicCtrl trong scene (fallback)
            MusicCtrl[] allMusic = FindObjectsOfType<MusicCtrl>();
            int sceneStoppedCount = 0;
            foreach (MusicCtrl music in allMusic)
            {
                if (music != null && music.gameObject.activeSelf)
                {
                    music.gameObject.SetActive(false);
                    sceneStoppedCount++;
                }
            }
            Debug.Log($"Stopped {sceneStoppedCount} music objects in scene");
            
            Debug.Log("All background music stop completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopAllBackgroundMusic: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Đảm bảo sound system được khởi tạo đầy đủ khi vào scene mới
    /// </summary>
    protected virtual void EnsureSoundSystemInitialized()
    {
        try
        {
            Debug.Log("=== ENSURING SOUND SYSTEM INITIALIZED ===");
            
            // Đảm bảo SoundSpawnerCtrl tồn tại
            if (this.ctrl == null)
            {
                Debug.Log("SoundSpawnerCtrl is null, trying to reload...");
                this.LoadSoundSpawnerCtrl();
                this.EnsureSoundSpawnerExists();
            }
            
            // Đảm bảo background music được khởi động
            if (this.bgMusic == null || !this.bgMusic.gameObject.activeSelf)
            {
                Debug.Log("Background music not found or inactive, starting it...");
                this.StartMusicBackground();
            }
            else
            {
                Debug.Log("Background music is already active");
            }
            
            // Áp dụng lại settings để đảm bảo volume đúng
            this.ApplyLoadedSettings();
            
            Debug.Log("Sound system initialization check completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in EnsureSoundSystemInitialized: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
}
