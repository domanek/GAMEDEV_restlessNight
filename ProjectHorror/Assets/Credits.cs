using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] private int nextLevelID;
    [SerializeField] private string caption;
    [SerializeField] private string subcaption;

    [SerializeField] private float animationTime;

    [SerializeField] private TMPro.TextMeshProUGUI leaveCreditText;

    private float timer;

    void Start()
    {
        leaveCreditText.alpha = 0f;
        AudioManager.instance.Play("credits");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3f) leaveCreditText.alpha = 1.0f;
        if (timer > animationTime) LeaveCredits();
        else if (leaveCreditText.alpha == 1.0f && Input.GetKey(KeyCode.Escape)) LeaveCredits();
    }

    void LeaveCredits()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        SceneManager.LoadScene(nextLevelID);
    }
}
