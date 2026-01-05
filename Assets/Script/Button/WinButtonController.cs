using UnityEngine;
using UnityEngine.SceneManagement;

public class WinButtonController : MonoBehaviour
{
    
    public void RestartGameDariAwal()
    {
        
        Time.timeScale = 1f;

        
        PlayerPrefs.SetInt("CurrentGameLevel", 1);
        PlayerPrefs.Save();

        
        SceneManager.LoadScene("MainGame");
    }

    
    public void KeMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}