using UnityEngine;
using System.Collections;

public class EnemyAttackSound : SaiMonoBehaviour
{
    [SerializeField] protected EnemyCtrl enemyCtrl;
    [SerializeField] protected SoundName attackSoundName = SoundName.EnemyPunch;
    [SerializeField] protected float attackSoundDelay = 0.5f; // Delay trước khi phát âm thanh (giây)
    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected Coroutine attackSoundCoroutine;

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

    /// <summary>
    /// Gọi khi Enemy bắt đầu tấn công
    /// </summary>
    public virtual void StartAttack()
    {
        if (isAttacking) return; // Đang tấn công rồi thì không tấn công nữa
        
        isAttacking = true;
        
        // Hủy coroutine cũ nếu có
        if (attackSoundCoroutine != null)
        {
            StopCoroutine(attackSoundCoroutine);
        }
        
        // Bắt đầu coroutine phát âm thanh với delay
        attackSoundCoroutine = StartCoroutine(PlayAttackSoundWithDelay());
        
        Debug.Log($"{enemyCtrl.GetName()} bắt đầu tấn công - sẽ phát âm thanh sau {attackSoundDelay}s");
    }

    /// <summary>
    /// Gọi khi Enemy kết thúc tấn công
    /// </summary>
    public virtual void EndAttack()
    {
        isAttacking = false;
        
        // Hủy coroutine nếu chưa phát âm thanh
        if (attackSoundCoroutine != null)
        {
            StopCoroutine(attackSoundCoroutine);
            attackSoundCoroutine = null;
        }
        
        Debug.Log($"{enemyCtrl.GetName()} kết thúc tấn công");
    }

    /// <summary>
    /// Coroutine phát âm thanh với delay
    /// </summary>
    protected virtual IEnumerator PlayAttackSoundWithDelay()
    {
        // Chờ delay
        yield return new WaitForSeconds(attackSoundDelay);
        
        // Kiểm tra xem vẫn đang tấn công không
        if (!isAttacking) yield break;
        
        // Phát âm thanh
        this.PlayAttackSound();
        
        // Reset trạng thái
        isAttacking = false;
        attackSoundCoroutine = null;
    }

    /// <summary>
    /// Phát âm thanh đánh
    /// </summary>
    protected virtual void PlayAttackSound()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("SoundManager.Instance is null, cannot play attack sound");
            return;
        }

        // Tạo SFX tại vị trí của Enemy
        SFXCtrl attackSfx = SoundManager.Instance.CreateSfx(this.attackSoundName);
        if (attackSfx != null)
        {
            attackSfx.transform.position = this.transform.position;
            attackSfx.gameObject.SetActive(true);
            
            AudioSource audioSource = attackSfx.AudioSource;
            if (audioSource != null)
            {
                if (audioSource.clip != null)
                {
                    Debug.Log($"{enemyCtrl.GetName()} phát âm thanh đánh {this.attackSoundName} tại vị trí {this.transform.position} - Clip: {audioSource.clip.name}");
                }
                else
                {
                    Debug.LogWarning($"{enemyCtrl.GetName()} không thể phát âm thanh {this.attackSoundName} - AudioSource không có clip!");
                }
            }
            else
            {
                Debug.LogWarning($"{enemyCtrl.GetName()} không thể phát âm thanh {this.attackSoundName} - AudioSource is null!");
            }
        }
        else
        {
            Debug.LogWarning($"Failed to create attack SFX: {this.attackSoundName}");
        }
    }

    /// <summary>
    /// Phát âm thanh đánh ngay lập tức (không delay)
    /// </summary>
    public virtual void PlayAttackSoundImmediately()
    {
        this.PlayAttackSound();
    }

    /// <summary>
    /// Set delay cho âm thanh đánh
    /// </summary>
    public virtual void SetAttackSoundDelay(float delay)
    {
        this.attackSoundDelay = delay;
        Debug.Log($"{enemyCtrl.GetName()} attack sound delay set to {delay}s");
    }

    /// <summary>
    /// Set loại âm thanh đánh
    /// </summary>
    public virtual void SetAttackSoundName(SoundName soundName)
    {
        this.attackSoundName = soundName;
        Debug.Log($"{enemyCtrl.GetName()} attack sound set to {soundName}");
    }

    /// <summary>
    /// Kiểm tra có đang tấn công không
    /// </summary>
    public virtual bool IsAttacking()
    {
        return this.isAttacking;
    }

    /// <summary>
    /// Dừng tấn công và hủy âm thanh
    /// </summary>
    public virtual void StopAttack()
    {
        this.EndAttack();
    }

    protected virtual void OnDisable()
    {
        // Dừng tất cả coroutine khi object bị disable
        if (attackSoundCoroutine != null)
        {
            StopCoroutine(attackSoundCoroutine);
            attackSoundCoroutine = null;
        }
        isAttacking = false;
    }
}
