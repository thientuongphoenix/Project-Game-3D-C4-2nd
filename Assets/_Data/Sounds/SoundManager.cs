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
        if (this.bgMusic == null) this.bgMusic = this.CreateMusic(this.bgName);
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
        MusicCtrl soundPrefab = (MusicCtrl)this.ctrl.Prefabs.GetByName(soundName.ToString());
        return this.CreateMusic(soundPrefab);
    }

    public virtual MusicCtrl CreateMusic(MusicCtrl musicPrefab)
    {
        MusicCtrl newMusic = (MusicCtrl)this.ctrl.Spawner.Spawn(musicPrefab, Vector3.zero);
        newMusic.AudioSource.volume = this.volumeMusic;
        this.AddMusic(newMusic);
        return newMusic;
    }

    public virtual void AddMusic(MusicCtrl newMusic)
    {
        if(this.listMusic.Contains(newMusic)) return;
        this.listMusic.Add(newMusic);
    }

    public virtual SFXCtrl CreateSfx(SoundName soundName)
    {
        SFXCtrl soundPrefab = (SFXCtrl)this.ctrl.Prefabs.GetByName(soundName.ToString());
        return this.CreateSfx(soundPrefab);
    }

    public virtual SFXCtrl CreateSfx(SFXCtrl sfxPrefab)
    {
        SFXCtrl newSound = (SFXCtrl)this.ctrl.Spawner.Spawn(sfxPrefab, Vector3.zero);
        //Điều này đảm bảo rằng sfx mới cũng được cập nhật giá trị volume, vì set volume chỉ ảnh hưởng tới sfx đã sinh ra rồi và được add vào listSfx.
        newSound.AudioSource.volume = this.volumeSfx;
        this.AddSfx(newSound);
        return newSound;
    }

    public virtual void AddSfx(SFXCtrl newSound)
    {
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
    }

    public virtual void VolumeSfxUpdating(float volume)
    {
        this.volumeSfx = volume;
        foreach(SFXCtrl sfxCtrl in this.listSfx)
        {
            sfxCtrl.AudioSource.volume = this.volumeSfx;
        }
    }
}
