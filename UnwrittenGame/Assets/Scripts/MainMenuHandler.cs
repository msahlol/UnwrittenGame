using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(sceneName: "MainGame");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}