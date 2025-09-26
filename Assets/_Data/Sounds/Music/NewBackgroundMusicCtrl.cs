using UnityEngine;

public class NewBackgroundMusicCtrl : MusicCtrl
{
    // Class này kế thừa từ MusicCtrl
    // Đã có sẵn AudioSource và loop = true
    // Có thể thêm logic riêng nếu cần
    
    public override string GetName()
    {
        return "NewBackgroundMusic";
    }
    
    protected override void Start()
    {
        base.Start();
        // Thêm logic riêng cho sound mới nếu cần
        Debug.Log("NewBackgroundMusicCtrl: Started");
    }
}
