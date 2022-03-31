using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject hud;
    public bool isPaused = false;

    public void UpdateSelectedAbility(int abilityIndex)
    {
        Debug.Log(abilityIndex);
        hud.transform.Find("Ability Select").GetComponent<TextMeshProUGUI>().text = (abilityIndex + 1).ToString();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
        pauseMenu.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
