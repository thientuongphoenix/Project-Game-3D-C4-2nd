using UnityEngine;

public class NewSFXCtrl : SFXCtrl
{
    // Class này kế thừa từ SFXCtrl
    // Đã có sẵn AudioSource
    // Có thể thêm logic riêng nếu cần
    
    public override string GetName()
    {
        return "NewSFX";
    }
    
    protected override void Start()
    {
        base.Start();
        // Thêm logic riêng cho SFX mới nếu cần
        Debug.Log("NewSFXCtrl: Started");
    }
}
