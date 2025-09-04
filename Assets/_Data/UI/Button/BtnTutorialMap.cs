using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnTutorialMap : BtnMapLock
{
    [Header("Tutorial Map Settings")]
    [SerializeField] protected string tutorialRequiredMap = ""; // Tutorial map luôn unlock
    
    protected override void Start()
    {
        base.Start();
        this.SetRequiredMap(tutorialRequiredMap); // Không cần map nào để unlock tutorial
    }
    
    protected override void ExecuteMapAction()
    {
        Debug.Log("Loading Tutorial Map...");
        SceneManager.LoadScene("Hai_SampleScene");
    }
    
    protected override void ShowLockMessage()
    {
        string message = "Tutorial Map is locked!";
        Debug.Log(message);
        
        if (this.lockText != null)
        {
            this.lockText.text = message;
            this.lockText.gameObject.SetActive(true);
            Invoke(nameof(HideLockMessage), 3f);
        }
    }
} 