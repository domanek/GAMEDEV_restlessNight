using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TransitionManager : MonoBehaviour
{
    public Transition[] transitions;

    public static TransitionManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);
    }

    public void StartTransition(string name)
    {
        Transition t = Array.Find(transitions, transition => transition.name == name);
        if (t == null)
        {
            Debug.LogWarning("TransitionEvent " + name + " was not found!");
            return;
        }

        StartCoroutine(RunTransition(t));
    }

    private IEnumerator RunTransition(Transition t)
    {
        Color tempColor = t.transitionImage.color;
        Color startingColor = tempColor;

        t.transitionImage.gameObject.SetActive(true);
        if (t.isFading)
        {
            while (tempColor.a >= 0.01)
            {
                tempColor.a -= (t.transitionSpeed * Time.deltaTime);
                t.transitionImage.color = tempColor;
                yield return null;
            }
            t.transitionImage.gameObject.SetActive(false);
        }
        else
        {
            while (tempColor.a <= 0.99)
            {
                tempColor.a += (t.transitionSpeed * Time.deltaTime);
                t.transitionImage.color = tempColor;
                yield return null;
            }
            t.transitionImage.gameObject.SetActive(false);
        }
        t.transitionImage.color = startingColor;
        yield return null;
    }
}

[System.Serializable]
public class Transition
{
    [Header("General Parameters")]
    public string name;
    public float transitionSpeed;
    public bool isFading;

    [Header("Image Parameters")]
    public Image transitionImage;
}
