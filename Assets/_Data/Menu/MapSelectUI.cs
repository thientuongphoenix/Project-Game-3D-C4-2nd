using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectUI : MonoBehaviour
{
    public void LoadTutorialMap()
    {
        // Reset quest trước khi load tutorial map
        this.ResetQuestsBeforeLoad();
        
        SceneManager.LoadScene("Hai_SampleScene");
    }

    public void LoadMap1()
    {
        // Reset quest trước khi load map 1
        this.ResetQuestsBeforeLoad();
        SceneManager.LoadScene("Hai_Map");
    }

    public void LoadMap2()
    {
        // Reset quest trước khi load map 2
        this.ResetQuestsBeforeLoad();
        SceneManager.LoadScene("Map2");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Hai_Menu");
    }
    
    /// <summary>
    /// Reset tất cả quest trước khi load map
    /// </summary>
    protected virtual void ResetQuestsBeforeLoad()
    {
        try
        {
            Debug.Log("=== RESETTING QUESTS BEFORE LOADING MAP ===");
            
            // Reset TowerQuestSystem nếu có
            if (TowerQuestSystem.Instance != null)
            {
                TowerQuestSystem.Instance.ResetAndReinitializeQuests();
                Debug.Log("TowerQuestSystem reset before map load!");
            }
            
            // Reset ItemGuideUI nếu có
            if (ItemGuideUI.Instance != null)
            {
                ItemGuideUI.Instance.ResetGuideState();
                Debug.Log("ItemGuideUI reset before map load!");
            }
            
            // Reset GameResultManager quests nếu có
            if (GameResultManager.Instance != null)
            {
                GameResultManager.Instance.ResetTutorialQuestsPublic();
                Debug.Log("GameResultManager quests reset before map load!");
            }
            
            Debug.Log("All quests have been reset before map load!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ResetQuestsBeforeLoad: {e.Message}");
        }
    }
}
