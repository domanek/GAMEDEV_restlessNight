using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    //HUD: load one instance to additive scene, contains all the player UI elements
    public static HUD instance = null;

    [SerializeField] GameObject pauseUI = null;
    [SerializeField] GameObject controllsUI = null;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        //DontDestroyOnLoad(this);
    }

    public void OpenPause()
    {
        pauseUI.SetActive(true);
    }

    public void ShowControlls(bool value)
    {
        this.controllsUI.SetActive(value);
        Debug.Log("controls ui: " + value);
    }
}
