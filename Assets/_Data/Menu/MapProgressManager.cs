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
        return completedMaps.Contains(mapName);
    }
    
    public virtual void CompleteMap(string mapName)
    {
        if (!completedMaps.Contains(mapName))
        {
            completedMaps.Add(mapName);
            SaveMapProgress();
            Debug.Log($"Map {mapName} completed!");
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