using UnityEngine;

public class Map1BackgroundMusicCtrl : MusicCtrl
{
    public override string GetName()
    {
        return "Map1";
    }
    
    protected override void Start()
    {
        base.Start();
        Debug.Log("Map1BackgroundMusicCtrl: Started");
    }
}

