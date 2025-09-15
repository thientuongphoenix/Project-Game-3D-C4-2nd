using UnityEngine;

/// <summary>
/// Hướng dẫn sử dụng EnemyAttackSound
/// Script này chỉ để tham khảo, không cần gắn vào GameObject
/// </summary>
public class EnemyAttackSoundGuide : MonoBehaviour
{
    [Header("=== HƯỚNG DẪN SỬ DỤNG ENEMY ATTACK SOUND ===")]
    [TextArea(10, 20)]
    public string guide = @"
🎯 CHỨC NĂNG:
- Enemy phát âm thanh đánh khi tấn công
- Có delay để đồng bộ với hoạt ảnh đánh
- Hỗ trợ 3D spatial audio

⚙️ CẤU HÌNH TRONG INSPECTOR:
EnemyAttackSound (Script):
├── Attack Sound Delay: 0.5    (delay trước khi phát âm - giây)
├── Attack Sound Name: EnemyPunch (loại âm thanh)
└── Is Attacking: false        (trạng thái đang tấn công)

🎮 CÁCH HOẠT ĐỘNG:
1. Enemy bắt đầu tấn công → StartAttack()
2. Chờ delay (0.5s mặc định)
3. Phát âm thanh EnemyPunch tại vị trí Enemy
4. Kết thúc tấn công → EndAttack()

🔧 ĐIỀU KHIỂN BẰNG CODE:
// Lấy EnemyCtrl
EnemyCtrl enemy = GetComponent<EnemyCtrl>();

// Bắt đầu tấn công (có delay)
enemy.EnemyAttackSound.StartAttack();

// Kết thúc tấn công
enemy.EnemyAttackSound.EndAttack();

// Phát âm thanh ngay lập tức (không delay)
enemy.EnemyAttackSound.PlayAttackSoundImmediately();

// Cấu hình delay khác nhau cho từng Enemy
enemy.EnemyAttackSound.SetAttackSoundDelay(0.3f); // 0.3 giây

// Thay đổi loại âm thanh
enemy.EnemyAttackSound.SetAttackSoundName(SoundName.BerettaM9Shot);

// Kiểm tra trạng thái
bool isAttacking = enemy.EnemyAttackSound.IsAttacking();

// Dừng tấn công
enemy.EnemyAttackSound.StopAttack();

🎵 CÁC LOẠI ÂM THANH CÓ SẴN:
- EnemyPunch: Tiếng đánh của Enemy
- BerettaM9Shot: Tiếng súng
- MagicSpell: Tiếng phép thuật
- Flame: Tiếng lửa
- EvilScream: Tiếng hét

⚡ TÍCH HỢP TỰ ĐỘNG:
- Tự động thêm vào EnemyCtrl
- Tự động gọi khi Enemy tấn công (AttackTower/AttackPlayer)
- Không cần setup thủ công

💡 LƯU Ý:
- Delay mặc định: 0.5 giây
- Có thể chỉnh delay khác nhau cho từng loại Enemy
- Âm thanh phát tại vị trí Enemy (3D sound)
- Tự động dừng khi Enemy chết
- Sử dụng object pooling để tối ưu performance

🔍 DEBUG:
- Xem Console để thấy log khi Enemy tấn công
- Kiểm tra trạng thái Is Attacking trong Inspector
- Test bằng cách gọi StartAttack() trong code
";

    [Header("=== TEST CONTROLS ===")]
    [SerializeField] protected EnemyCtrl testEnemy;
    [SerializeField] protected float testDelay = 0.5f;

    [ContextMenu("Test Start Attack")]
    public void TestStartAttack()
    {
        if (testEnemy != null && testEnemy.EnemyAttackSound != null)
        {
            testEnemy.EnemyAttackSound.StartAttack();
            Debug.Log("Test: Started attack with sound delay");
        }
    }

    [ContextMenu("Test Play Immediately")]
    public void TestPlayImmediately()
    {
        if (testEnemy != null && testEnemy.EnemyAttackSound != null)
        {
            testEnemy.EnemyAttackSound.PlayAttackSoundImmediately();
            Debug.Log("Test: Played attack sound immediately");
        }
    }

    [ContextMenu("Test Set Delay")]
    public void TestSetDelay()
    {
        if (testEnemy != null && testEnemy.EnemyAttackSound != null)
        {
            testEnemy.EnemyAttackSound.SetAttackSoundDelay(testDelay);
            Debug.Log($"Test: Set attack delay to {testDelay}s");
        }
    }
}
