using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnMap1 : BtnMapLock
{
    [Header("Map 1 Settings")]
    [SerializeField] protected string map1RequiredMap = "SampleScene"; // Cần hoàn thành Tutorial để unlock Map 1
    
    protected override void Start()
    {
        base.Start();
        this.SetRequiredMap(map1RequiredMap);
    }
    
    protected override void ExecuteMapAction()
    {
        Debug.Log("Loading Map 1...");
        SceneManager.LoadScene("Hai_Map");
    }
    
    protected override void ShowLockMessage()
    {
        string message = $"Map 1 is locked!\nComplete Tutorial to unlock";
        Debug.Log(message);
        
        if (this.lockText != null)
        {
            this.lockText.text = message;
            this.lockText.gameObject.SetActive(true);
            Invoke(nameof(HideLockMessage), 3f);
        }
    }
} 