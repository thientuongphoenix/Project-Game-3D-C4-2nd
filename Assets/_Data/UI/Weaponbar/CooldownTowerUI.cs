using UnityEngine;
using UnityEngine.UI;

public class CooldownTowerUI : MonoBehaviour
{
    [Header("Image fill dạng cooldown")]
    public Image cooldownImage; // Image fill dạng radial
    private float cooldownTime;
    private float cooldownTimer;
    private bool isCooldown;

    // Gọi hàm này khi đặt thành công súng/trap
    public void StartCooldown(float time)
    {
        cooldownTime = time;
        cooldownTimer = time;
        isCooldown = true;
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
            SetAlpha(0.6f); // Hiện image cooldown
        }
    }

    void Update()
    {
        if (!isCooldown || cooldownImage == null) return;

        cooldownTimer -= Time.deltaTime;
        cooldownImage.fillAmount = Mathf.Clamp01(cooldownTimer / cooldownTime);

        if (cooldownTimer <= 0f)
        {
            isCooldown = false;
            cooldownImage.fillAmount = 0f;
            SetAlpha(0f); // Ẩn image cooldown
        }
    }

    private void SetAlpha(float alpha)
    {
        var color = cooldownImage.color;
        color.a = alpha;
        cooldownImage.color = color;
    }
}
