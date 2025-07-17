using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectUI : MonoBehaviour
{
    public void LoadTutorialMap()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadMap1()
    {
        SceneManager.LoadScene("Hai_Map");
    }

    public void LoadMap2()
    {
        SceneManager.LoadScene("Map2");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Hai_Menu");
    }
}
