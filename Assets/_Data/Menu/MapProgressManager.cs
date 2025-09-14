using System.Collections.Generic;
using UnityEngine;

public class MapProgressManager : SaiSingleton<MapProgressManager>
{
    [Header("Map Progress")]
    [SerializeField] protected List<string> completedMaps = new List<string>();
    
    protected override void Start()
    {
        base.Start();
        this.LoadMapProgress();
        
        // Đảm bảo MapProgressManager không bị destroy khi chuyển scene
        DontDestroyOnLoad(gameObject);
        Debug.Log("MapProgressManager: DontDestroyOnLoad set");
    }
    
    protected virtual void LoadMapProgress()
    {
        // Load từ PlayerPrefs
        string savedMaps = PlayerPrefs.GetString("CompletedMaps", "");
        if (!string.IsNullOrEmpty(savedMaps))
        {
            string[] maps = savedMaps.Split(',');
            foreach (string map in maps)
            {
                if (!string.IsNullOrEmpty(map) && !completedMaps.Contains(map))
                {
                    completedMaps.Add(map);
                }
            }
        }
        
        Debug.Log($"Loaded {completedMaps.Count} completed maps: {string.Join(", ", completedMaps)}");
    }
    
    protected virtual void SaveMapProgress()
    {
        string mapsString = string.Join(",", completedMaps);
        PlayerPrefs.SetString("CompletedMaps", mapsString);
        PlayerPrefs.Save();
        Debug.Log($"Saved map progress: {mapsString}");
    }
    
    public virtual bool IsMapCompleted(string mapName)
    {
        bool isCompleted = completedMaps.Contains(mapName);
        Debug.Log($"IsMapCompleted('{mapName}'): {isCompleted} - Current maps: {string.Join(", ", completedMaps)}");
        return isCompleted;
    }
    
    public virtual void CompleteMap(string mapName)
    {
        Debug.Log($"=== COMPLETING MAP: {mapName} ===");
        Debug.Log($"Before: completedMaps contains '{mapName}': {completedMaps.Contains(mapName)}");
        
        if (!completedMaps.Contains(mapName))
        {
            completedMaps.Add(mapName);
            SaveMapProgress();
            Debug.Log($"Map {mapName} completed and added to list!");
            Debug.Log($"After: completedMaps now contains: {string.Join(", ", completedMaps)}");
        }
        else
        {
            Debug.Log($"Map {mapName} already completed!");
        }
    }
    
    public virtual void ResetAllProgress()
    {
        completedMaps.Clear();
        PlayerPrefs.DeleteKey("CompletedMaps");
        PlayerPrefs.Save();
        Debug.Log("All map progress reset!");
    }
    
    public virtual List<string> GetCompletedMaps()
    {
        return new List<string>(completedMaps);
    }
    
    public virtual int GetCompletedMapCount()
    {
        return completedMaps.Count;
    }
} 