using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public static DoorController instance;

    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private float moveDistance = 90f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private string solutionString = "3215";
    [SerializeField] private string fakeSolutionString = "1235";

    private string solutionInput = "";
    private bool solved = false;
    private bool fakeEntered = false;
    private int timesTried = 0;

    private bool playedHint = false;
    private bool playedHint2 = false;

    private void Start()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!solved) CheckSolution();
    }

    private void CheckSolution()
    {
        if (!fakeEntered && (fakeSolutionString == solutionInput))
        {
            TextManager.instance.StartTextEvent("code2");

            fakeEntered = true;
        }
        else if (solutionString == solutionInput)
        {
            solved = true;
            AudioManager.instance.Play("unlocked");
            StartCoroutine(OpenDoors());
        }
        else if (solutionInput.Length >= solutionString.Length)
        {
            solutionInput = "";
            AudioManager.instance.Play("denied");
            timesTried++;
        }

        if (timesTried == 8 && !playedHint)
        {
            playedHint = true;
            TextManager.instance.StartTextEvent("keypadhint");
        }
        if (timesTried == 12 && !playedHint2)
        {
            playedHint2 = true;
            TextManager.instance.StartTextEvent("keypadhint2");
        }
    }

    public void ButtonHasBeenPressed(string identifier)
    {
        solutionInput += identifier;
        Debug.Log("Current solutionInput: " + solutionInput);
    }

    private IEnumerator OpenDoors()
    {
        if (leftDoor != null) leftDoor.LeanRotateAround(new Vector3(0, -1, 0), moveDistance, moveDuration);
        if (rightDoor != null) rightDoor.LeanRotateAround(new Vector3(0, 1, 0), moveDistance, moveDuration);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.Play("doorunlocked");
    }
}
