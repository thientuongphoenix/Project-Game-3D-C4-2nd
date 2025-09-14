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
        //this.StartMusicBackground();
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
    }

    protected virtual void LoadSoundSpawnerCtrl()
    {
        if (this.ctrl != null) return;
        this.ctrl = GameObject.FindAnyObjectByType<SoundSpawnerCtrl>();
        Debug.Log(transform.name + ": LoadSoundSpawnerCtrl", gameObject);
    }

    public virtual void StartMusicBackground()
    {
        if (this.bgMusic == null) 
        {
            this.bgMusic = this.CreateMusic(this.bgName);
            if (this.bgMusic == null)
            {
                Debug.LogError("SoundManager: Failed to create background music");
                return;
            }
        }
        this.bgMusic.gameObject.SetActive(true);
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
    }

    public virtual MusicCtrl CreateMusic(SoundName soundName)
    {
        if (this.ctrl == null)
        {
            //Debug.LogError("SoundManager: ctrl is null, trying to reload...");
            this.LoadSoundSpawnerCtrl();
            if (this.ctrl == null)
            {
                Debug.LogError("SoundManager: Failed to load SoundSpawnerCtrl");
                return null;
            }
        }
        
        MusicCtrl soundPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName(soundName.ToString());
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
        if(this.listMusic.Contains(newMusic)) return;
        this.listMusic.Add(newMusic);
    }

    public virtual SFXCtrl CreateSfx(SoundName soundName)
    {
        if (this.ctrl == null)
        {
            //Debug.LogError("SoundManager: ctrl is null, trying to reload...");
            this.LoadSoundSpawnerCtrl();
            if (this.ctrl == null)
            {
                Debug.LogError("SoundManager: Failed to load SoundSpawnerCtrl");
                return null;
            }
        }
        
        SFXCtrl soundPrefab = (SFXCtrl)this.ctrl.Prefabs.GetByName(soundName.ToString());
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
        foreach(MusicCtrl musicCtrl in this.listMusic)
        {
            musicCtrl.AudioSource.volume = this.volumeMusic;
        }
        // Lưu settings khi volume thay đổi
        this.SaveSettings();
    }

    public virtual void VolumeSfxUpdating(float volume)
    {
        this.volumeSfx = volume;
        if (this.listSfx == null)
        {
            Debug.LogWarning("SoundManager: listSfx is null, initializing...");
            this.listSfx = new System.Collections.Generic.List<SFXCtrl>();
            return;
        }
        
        foreach(SFXCtrl sfxCtrl in this.listSfx)
        {
            if (sfxCtrl != null && sfxCtrl.AudioSource != null)
            {
                sfxCtrl.AudioSource.volume = this.volumeSfx;
            }
        }
        // Lưu settings khi volume thay đổi
        this.SaveSettings();
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
        // Áp dụng cho music
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
        
        // Áp dụng cho SFX
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
}
