using UnityEngine;

public class BtnSelectMap : ButttonAbstract
{
    protected override void OnClick()
    {
        // Khôi phục SFX về bình thường trước khi chuyển scene
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.VolumeSfxUpdating(1f);
            //Debug.Log("Đã khôi phục SFX về bình thường");
        }
        
        string mapName = "MapSelect_Hai";
        UnityEngine.SceneManagement.SceneManager.LoadScene(mapName);
    }
}
