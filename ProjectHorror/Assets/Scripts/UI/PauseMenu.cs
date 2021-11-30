using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape)) gameObject.SetActive(false);
    }

    public void OpenSettings()
    {

    }

    public void OpenMainMenu()
    {
        AudioManager.instance.StopPlayAll();
        TextManager.instance.StopAllTextEvents();
        SceneManager.LoadScene(0);
        gameObject.SetActive(false);
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1.0f;
        AudioManager.instance.StopPlayAll();
        TextManager.instance.StopAllTextEvents();
        SceneManager.LoadScene(1);
        gameObject.SetActive(false);
    }
}
