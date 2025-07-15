using UnityEngine;
using System.Collections;

public class GameResultManager : MonoBehaviour
{
    
    void Start()
    {
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

    protected virtual void Init()
    {
        this.player = PlayerCtrl.Instance;
        this.core = FindObjectOfType<CoreCtrl>();
        this.winPanel = GameObject.Find("WinPanel");
        this.losePanel = GameObject.Find("LosePanel");
        if (this.winPanel != null) this.winPanel.SetActive(false);
        if (this.losePanel != null) this.losePanel.SetActive(false);
    }

    protected virtual void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    protected virtual void CheckLoseCondition()
    {
        if (isLose || isGameEnded) return;
        if (IsPlayerDead() || IsCoreDead())
        {
            isLose = true;
            StartCoroutine(ShowLosePanelAfterDelay(3f));
        }
    }

    protected virtual void CheckWinCondition()
    {
        if (isWin || isGameEnded) return;
        if (timer >= defendTime)
        {
            isWin = true;
            ShowWinPanel();
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
        isGameEnded = true;
        if (winPanel != null) winPanel.SetActive(true);
        if (losePanel != null) losePanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        // Có thể bổ sung hiệu ứng, âm thanh,... ở đây
    }

    protected virtual void ShowLosePanel()
    {
        isGameEnded = true;
        if (losePanel != null) losePanel.SetActive(true);
        if (winPanel != null) winPanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        // Có thể bổ sung hiệu ứng, âm thanh,... ở đây
    }
}
