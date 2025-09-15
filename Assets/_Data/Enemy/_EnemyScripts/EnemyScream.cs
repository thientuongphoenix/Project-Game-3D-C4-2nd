using UnityEngine;

public class EnemyScream : SaiMonoBehaviour
{
    [SerializeField] protected EnemyCtrl enemyCtrl;
    [SerializeField] protected float minScreamInterval = 5f; // Thời gian tối thiểu giữa các lần hét (giây)
    [SerializeField] protected float maxScreamInterval = 15f; // Thời gian tối đa giữa các lần hét (giây)
    [SerializeField] protected float nextScreamTime = 0f; // Thời điểm hét tiếp theo
    [SerializeField] protected bool canScream = true; // Có thể hét hay không
    [SerializeField] protected SoundName screamSoundName = SoundName.EvilScream;

    protected override void Start()
    {
        base.Start();
        this.ScheduleNextScream();
    }

    protected virtual void Update()
    {
        if (!canScream) return;
        if (enemyCtrl == null) return;
        if (enemyCtrl.EnemyDamageReceiver != null && enemyCtrl.EnemyDamageReceiver.IsDead()) return;

        // Kiểm tra xem đã đến lúc hét chưa
        if (Time.time >= nextScreamTime)
        {
            this.Scream();
            this.ScheduleNextScream();
        }
    }

    protected virtual void Scream()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("SoundManager.Instance is null, cannot play scream sound");
            return;
        }

        // Tạo SFX tại vị trí của Enemy
        SFXCtrl screamSfx = SoundManager.Instance.CreateSfx(this.screamSoundName);
        if (screamSfx != null)
        {
            screamSfx.transform.position = this.transform.position;
            screamSfx.gameObject.SetActive(true);
            Debug.Log($"{enemyCtrl.GetName()} đang hét tại vị trí {this.transform.position}");
        }
        else
        {
            Debug.LogWarning($"Failed to create scream SFX: {this.screamSoundName}");
        }
    }

    protected virtual void ScheduleNextScream()
    {
        // Tính thời gian hét tiếp theo (ngẫu nhiên trong khoảng 5-15 giây)
        float randomInterval = Random.Range(minScreamInterval, maxScreamInterval);
        nextScreamTime = Time.time + randomInterval;
        
        Debug.Log($"{enemyCtrl.GetName()} sẽ hét lần tiếp theo sau {randomInterval:F1} giây");
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadEnemyCtrl();
    }

    protected virtual void LoadEnemyCtrl()
    {
        if (this.enemyCtrl != null) return;
        this.enemyCtrl = GetComponentInParent<EnemyCtrl>();
        if (this.enemyCtrl == null)
        {
            this.enemyCtrl = GetComponent<EnemyCtrl>();
        }
        Debug.Log(transform.name + ": LoadEnemyCtrl", gameObject);
    }

    // Có thể tạm dừng hoặc tiếp tục hét
    public virtual void SetCanScream(bool canScream)
    {
        this.canScream = canScream;
        if (canScream)
        {
            this.ScheduleNextScream();
        }
    }

    // Hét ngay lập tức (không cần chờ interval)
    public virtual void ForceScream()
    {
        this.Scream();
        this.ScheduleNextScream();
    }

    // Reset thời gian hét tiếp theo
    public virtual void ResetScreamTimer()
    {
        this.ScheduleNextScream();
    }
}
