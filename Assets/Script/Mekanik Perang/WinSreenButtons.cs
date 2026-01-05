using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenButtons : MonoBehaviour
{
    public void BackToMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

    public void RestartGame()
    {
        
        PlayerPrefs.SetInt("CurrentGameLevel", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGame"); 
    }
}