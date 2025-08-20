using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerCtrl : SaiSingleton<PlayerCtrl>
{
    [SerializeField] protected vThirdPersonController thirdPersonCtrl;
    public vThirdPersonController ThirdPersonController => thirdPersonCtrl;


    [SerializeField] protected vThirdPersonCamera thirdPersonCamera;
    public vThirdPersonCamera ThirdPersonCamera => thirdPersonCamera;

    [SerializeField] protected CrosshairPointer crosshairPointer;
    public CrosshairPointer CrosshairPointer => crosshairPointer;

    [SerializeField] protected Rig aimingRig;
    public Rig AimingRig => aimingRig;

    [SerializeField] protected Animator animator;
    public Animator Animator => animator;

    [SerializeField] protected Weapons weapons;
    public Weapons Weapons => weapons;

    [SerializeField] protected LevelAbstract level;
    public LevelAbstract Level => level;

    [SerializeField] protected PlayerDamageReceiver playerDamageReceiver;
    public PlayerDamageReceiver PlayerDamageReceiver => playerDamageReceiver;

    [SerializeField] protected PlayerMana playerMana;
    public PlayerMana PlayerMana => playerMana;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadThirdPersonCtrl();
        this.LoadThirdPersonCamera();
        this.LoadCrosshairPointer();
        this.LoadAimingRig();
        this.LoadAnimator();
        this.LoadWeapons();
        this.LoadLevel();
        this.LoadPlayerDamageReceiver();
        this.LoadPlayerMana();
    }

    protected virtual void LoadPlayerMana()
    {
        if (this.playerMana != null) return;
        this.playerMana = GetComponentInChildren<PlayerMana>();
        Debug.Log(transform.name + ": LoadPlayerMana", gameObject);
    }

    protected virtual void LoadPlayerDamageReceiver()
    {
        if (this.playerDamageReceiver != null) return;
        this.playerDamageReceiver = GetComponentInChildren<PlayerDamageReceiver>();
        Debug.Log(transform.name + ": LoadPlayerDamageReceiver", gameObject);
    }

    protected virtual void LoadLevel()
    {
        if (this.level != null) return;
        this.level = GetComponentInChildren<LevelAbstract>();
        Debug.Log(transform.name + ": LoadLevel", gameObject);
    }

    protected virtual void LoadAimingRig()
    {
        if(this.aimingRig != null) return;
        this.aimingRig = transform.Find("Model").Find("AimingRig").GetComponent<Rig>();
        Debug.Log(transform.name + " LoadAimingRig", gameObject);
    }

    protected virtual void LoadCrosshairPointer()
    {
        if (this.crosshairPointer != null) return;
        this.crosshairPointer = GetComponentInChildren<CrosshairPointer>();
        Debug.Log(transform.name + ": LoadCrosshairPointer", gameObject);
    }

    protected virtual void LoadThirdPersonCtrl()
    {
        if (this.thirdPersonCtrl != null) return;
        this.thirdPersonCtrl = GetComponent<vThirdPersonController>();
        Debug.Log(transform.name + ": LoadThirPersonCtrl", gameObject);
    }    


    protected virtual void LoadThirdPersonCamera()
    {
        if (this.thirdPersonCamera != null) return;
        this.thirdPersonCamera = GameObject.FindAnyObjectByType<vThirdPersonCamera>();
        this.thirdPersonCamera.rightOffset = 0.6f;
        this.thirdPersonCamera.defaultDistance = 1.3f;
        // this.thirdPersonCamera.height = 1.4f;
        // this.thirdPersonCamera.yMinLimit = -40f;
        // this.thirdPersonCamera.yMaxLimit = 80f;
        Debug.Log(transform.name + ": LoadThirdPersonCamera", gameObject);
    }

    protected virtual void LoadAnimator()
    {
        if(this.animator != null) return;
        this.animator = GetComponentInChildren<Animator>();
        Debug.Log(transform.name + ": LoadAnimator", gameObject);
    }

    protected virtual void LoadWeapons()
    {
        if (this.weapons != null) return;
        this.weapons = GetComponentInChildren<Weapons>();
        Debug.Log(transform.name + ": LoadWeapons", gameObject);
    }

    protected virtual void Start()
    {
        // Subscribe to death event
        if (this.playerDamageReceiver != null)
        {
            this.playerDamageReceiver.OnPlayerDeath += HandlePlayerDeath;
        }
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe from death event
        if (this.playerDamageReceiver != null)
        {
            this.playerDamageReceiver.OnPlayerDeath -= HandlePlayerDeath;
        }
    }

    protected virtual void HandlePlayerDeath()
    {
        DisablePlayerComponents();
        Debug.Log(transform.name + ": Player death handled - Components disabled", gameObject);
    }

    protected virtual void DisablePlayerComponents()
    {
        // Disable GameObject chứa PlayerDamageReceiver
        if (this.playerDamageReceiver != null)
        {
            this.playerDamageReceiver.gameObject.SetActive(false);
        }

        // Disable GameObject chứa EnemyTargetable
        var enemyTargetable = GetComponentInChildren<EnemyTargetablePlayer>();
        if (enemyTargetable != null)
        {
            enemyTargetable.gameObject.SetActive(false);
        }

        Debug.Log(transform.name + ": PlayerDamageReceiver and EnemyTargetable GameObjects disabled", gameObject);
    }

    // Hàm để enable lại components khi resurrect
    public virtual void EnablePlayerComponents()
    {
        // Enable GameObject chứa PlayerDamageReceiver
        if (this.playerDamageReceiver != null)
        {
            this.playerDamageReceiver.gameObject.SetActive(true);
        }

        // Enable GameObject chứa EnemyTargetable
        var enemyTargetable = GetComponentInChildren<EnemyTargetablePlayer>();
        if (enemyTargetable != null)
        {
            enemyTargetable.gameObject.SetActive(true);
        }

        Debug.Log(transform.name + ": PlayerDamageReceiver and EnemyTargetable GameObjects enabled", gameObject);
    }
}
