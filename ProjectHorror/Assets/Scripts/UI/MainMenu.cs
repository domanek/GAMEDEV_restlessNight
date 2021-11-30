using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject controlsUI;

    private void Start()
    {
        AudioManager.instance.StopPlay("credits");
        AudioManager.instance.Play("theme2");
        Cursor.visible = true;
    }

    private void Update()
    {
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    public void StartGame(int id)
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Invoke("StartScene", 0.25f); 
    }

    private void StartScene()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        settingsUI.SetActive(true);
    }

    public void OpenControls()
    {
        controlsUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsUI.SetActive(false);
    }

    public void CloseControls()
    {
        controlsUI.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
              Application.Quit();
#endif
    }
}
