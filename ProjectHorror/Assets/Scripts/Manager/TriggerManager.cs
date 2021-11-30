using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerManager : MonoBehaviour
{
    public TriggerEvent[] triggerEvents;

    public static TriggerManager instance;

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

    public void StartTriggerEvent(string name)
    {
        TriggerEvent t = Array.Find(triggerEvents, trigger => trigger.name == name);
        if (t == null)
        {
            Debug.LogWarning("TriggerEvent " + name + " was not found!");
            return;
        }

        if (!t.isRunning)
        {
            if (!t.isSpawningObject)
            {
                if (t.isUsingLights && t.switchOffLight) StartCoroutine(RunTriggerEventBulb(t));
            }
            else
            {
                StartCoroutine(RunTriggerEventLightAndSpawn(t));
            }
        }

    }
    public void StartTriggerEvent(string name, GameObject connectedObj, GameObject connectedObjSpawn, GameObject connectedLight)
    {
        // Try to find TriggerEvent
        TriggerEvent t = Array.Find(triggerEvents, trigger => trigger.name == name);
        if (t == null)
        {
            Debug.LogWarning("TriggerEvent " + name + " was not found!");
            return;
        }

        // Setup Trigger Components - Understand Trigger and Play right Event
        LightFlicker hasLightflicker = connectedLight.GetComponent<LightFlicker>();
        Enemy hasEnemyScript = connectedObj.GetComponent<Enemy>();
        if (hasLightflicker != null) t.light = hasLightflicker;
        if (connectedObj != null && hasEnemyScript != null)
        {
            t.obj = connectedObj;
            t.objPosition = connectedObjSpawn.transform;
        }
        else if(connectedObj != null) t.objPosition = connectedObjSpawn.transform;

        // Run Trigger if it is not already running
        if (!t.isRunning)
        {
            if (!t.isSpawningObject)
            {
                if (t.isUsingLights && t.switchOffLight)
                {
                    //if (t.light == null) t.light = connectedObj.GetComponent<LightFlicker>();
                    StartCoroutine(RunTriggerEventBulb(t));
                }
                else
                {
                    StartCoroutine(RunTriggerEventLightAndSpawn(t));
                }
            }
            else
            {
                if (hasEnemyScript != null)
                {
                    StartCoroutine(RunTriggerEventEnemyAppearing(t));
                }
                else StartCoroutine(RunTriggerEventLightAndSpawn(t));
            }
        }

    }

    private IEnumerator RunTriggerEventEnemyAppearing(TriggerEvent t)
    {
        t.isRunning = true;

        if (t.isUsingLights) t.light.allowFlicker = true;
        GameObject objInstance = null;
        if (t.isSpawningObject)
        {
            objInstance = Instantiate(t.obj, t.objPosition);
        }
        if (t.switchOffLight) t.light.SwitchOffLights();
        foreach (string clip in t.clipNames)
        {
            AudioManager.instance.Play(clip);
        }

        yield return new WaitForSeconds(t.duration);

        if (t.isUsingLights) t.light.allowFlicker = false;
        if (t.isSpawningObject && objInstance != null) Destroy(objInstance);
        foreach (string clip in t.clipNames)
        {
            AudioManager.instance.StopPlay(clip);
        }

        t.isRunning = false;
    }

    private IEnumerator RunTriggerEventLightAndSpawn(TriggerEvent t)
    {
        t.isRunning = true;

        if (t.isUsingLights) t.light.allowFlicker = true;
        GameObject objInstance = null;
        if (t.isSpawningObject)
        {
            objInstance = Instantiate(t.obj, t.objPosition);
        }
        if (t.switchOffLight) t.light.SwitchOffLights();
        foreach (string clip in t.clipNames)
        {
            AudioManager.instance.Play(clip);
        }

        yield return new WaitForSeconds(t.duration);

        if (t.isUsingLights) t.light.allowFlicker = false;
        if (t.isSpawningObject && objInstance != null) Destroy(objInstance);
        foreach (string clip in t.clipNames)
        {
            AudioManager.instance.StopPlay(clip);
        }

        t.isRunning = false;
    }

    private IEnumerator RunTriggerEventSpawn(TriggerEvent t)
    {
        t.isRunning = true;

        PlayEventAudio(t);
        GameObject objInstance = null;
        if (t.isSpawningObject) objInstance = Instantiate(t.obj, t.objPosition);

        yield return new WaitForSeconds(t.duration);

        StopEventAudio(t);
        if (t.isSpawningObject && objInstance != null) Destroy(objInstance);

        t.isRunning = false;
    }

    private IEnumerator RunTriggerEventBulb(TriggerEvent t)
    {
        t.isRunning = true;

        AudioManager.instance.Play(t.clipNames[0]);
        if (t.isUsingLights)
        {
            t.light.SetIntensity(3f);
            t.light.allowFlicker = true;
        }

        yield return new WaitForSeconds(t.duration);

        AudioManager.instance.StopPlay(t.clipNames[0]);
        AudioManager.instance.PlayWithPitch(t.clipNames[1], UnityEngine.Random.Range(0.9f, 1.1f));
        if (t.isUsingLights) t.light.allowFlicker = false;
        if (t.switchOffLight) t.light.SwitchOffLights();

        t.isRunning = false;
    }

    private void PlayEventAudio(TriggerEvent t)
    {
        foreach (string clip in t.clipNames)
        {
            AudioManager.instance.Play(clip);
        }
    }

    private void StopEventAudio(TriggerEvent t)
    {
        foreach (string clip in t.clipNames)
        {
            AudioManager.instance.StopPlay(clip);
        }
    }
}

[System.Serializable]
public class TriggerEvent
{
    [Header("Name and Duration of TriggerEvent")]
    public string name;
    public float duration;

    [Header("Options for TriggerEvent")]
    public bool isUsingLights = false;
    public bool switchOffLight = false;
    public bool isSpawningObject = false;

    [Header("TriggerEvent: Lightflicker")]
    public LightFlicker light;

    [Header("TriggerEvent: Object")]
    public GameObject obj;
    public Transform objPosition;

    [Header("TriggerEvent: Audio")]
    public string[] clipNames;

    [HideInInspector]
    public bool isRunning = false;
}

