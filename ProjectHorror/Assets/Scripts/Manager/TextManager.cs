using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    public Text[] texts;

    public static TextManager instance;

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

    public void StopAllTextEvents()
    {
        StopAllCoroutines();
        foreach(Text t in texts)
        {
            t.textUI.text = "";
        }
    }

    public void StartTextEvent(string name)
    {
        Text t = Array.Find(texts, text => text.name == name);
        if (t == null)
        {
            Debug.LogWarning("TextEvent " + name + " was not found!");
            return;
        }

        if (!t.isRunning) StartCoroutine(RunTextEvent(t));
    }

    private IEnumerator RunTextEvent(Text t)
    {
        t.isRunning = true;

        t.textUI.text = "";
        t.textUI.color = t.textColor;
        t.textUI.font = t.textFont;

        t.textUI.gameObject.SetActive(true);
        foreach (char c in t.text.ToCharArray())
        {
            if (t.usingAudio)
            {
                if (t.usingPitch) AudioManager.instance.PlayWithPitch(t.clipName, UnityEngine.Random.Range(0.5f, 0.8f));
                else AudioManager.instance.Play(t.clipName);
            }
            t.textUI.text += c;
            yield return new WaitForSeconds(t.textSpeed);
            if (t.usingAudio) AudioManager.instance.StopPlay(t.clipName);
        }
        if (t.usingAudio) AudioManager.instance.StopPlay(t.clipName);
        yield return new WaitForSeconds(t.durationOfText);

        t.textUI.gameObject.SetActive(false);

        t.isRunning = false;
    }
}

[System.Serializable]
public class Text
{
    [Header("General Parameters")]
    public string name;
    public float durationOfText;
    public float textSpeed = 0.05f;

    [Header("Audio Parameters")]
    public bool usingAudio;
    public bool usingPitch;
    public string clipName;

    [Header("UI Element Parameters")]
    public TMPro.TextMeshProUGUI textUI;
    public Vector3 textUIPosition;

    [Header("Text Parameters")]
    public string text;
    public Color textColor;
    public TMPro.TMP_FontAsset textFont;

    [HideInInspector]
    public bool isRunning;
}
