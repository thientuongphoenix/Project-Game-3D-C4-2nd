using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnMap2 : BtnMapLock
{
    [Header("Map 2 Settings")]
    [SerializeField] protected string map2RequiredMap = "Hai_Map"; // Cần hoàn thành Map 1 để unlock Map 2
    
    protected override void Start()
    {
        base.Start();
        this.SetRequiredMap(map2RequiredMap);
    }
    
    protected override void ExecuteMapAction()
    {
        Debug.Log("Loading Map 2...");
        SceneManager.LoadScene("Map2");
    }
    
    protected override void ShowLockMessage()
    {
        string message = $"Map 2 is locked!\nComplete Map 1 to unlock";
        Debug.Log(message);
        
        if (this.lockText != null)
        {
            this.lockText.text = message;
            this.lockText.gameObject.SetActive(true);
            Invoke(nameof(HideLockMessage), 3f);
        }
    }
} 