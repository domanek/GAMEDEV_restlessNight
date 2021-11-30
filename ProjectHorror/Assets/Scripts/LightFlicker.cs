using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public bool allowFlicker = false;

    [Header("Light setup parameters")]
    [SerializeField] private bool searchLightInChildren;
    [SerializeField] private bool searchForMultipleLights;

    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    [SerializeField] private Light light;
    [SerializeField] private Light[] lights;
    [Tooltip("Minimum random light intensity")]
    [SerializeField] private float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    private float maxIntensity;

    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 5;

    private int smoothingStart = 5;
    private float minIntensityStart;
    [SerializeField] private float[] lightIntensities;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> smoothQueue;
    float lastSum = 0;

    private bool hasBeenSwitchedOff = false;
    private bool isFlickering = false;

    public void StartFlickerOnce(float amount)
    {
        if (!isFlickering) StartCoroutine(StartFlicker(amount));
    }
    private IEnumerator StartFlicker(float strength)
    {
        minIntensity += strength;
        smoothing = 2;

        yield return new WaitForSeconds(2f);

        smoothing = smoothingStart;
        minIntensity = minIntensityStart;
    }   

    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;

        if (searchForMultipleLights)
        {

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = lightIntensities[i];
            }
        }
    }

    private void Awake()
    {
        if (light == null)
        {
            // External or internal light?
            if (searchLightInChildren)
            {
                if (searchForMultipleLights) lights = GetComponentsInChildren<Light>();
                else light = GetComponentInChildren<Light>();
            }
            else
            {
                if (searchForMultipleLights) lights = GetComponents<Light>();
                else light = GetComponent<Light>();
            }
        }
    }

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);

        if (searchForMultipleLights)
        {
            maxIntensity = 1f;
            lightIntensities = new float[lights.Length];

            for (int i = 0; i < lights.Length; i++)
            {
                lightIntensities[i] = lights[i].intensity;
            }
        }
        else maxIntensity = light.intensity;

        minIntensityStart = minIntensity;
    }

    void Update()
    {
        if (light == null && lights == null || !allowFlicker)
        {
            if (!hasBeenSwitchedOff) Reset();
            return;
        }

        // pop off an item if too big
        while (smoothQueue.Count >= smoothing && smoothQueue.Count != 0)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        // generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        // calculate new smoothed average
        float currVal = lastSum / (float)smoothQueue.Count;

        // change light intensity
        if (searchForMultipleLights)
        {
            foreach (Light l in lights)
            {
                l.intensity = currVal;
            }
        }
        else light.intensity = currVal;
    }

    public void SwitchOffLights()
    {
        hasBeenSwitchedOff = true;
        if (searchForMultipleLights)
        {
            foreach(Light l in lights)
            {
                l.intensity = 0f;
            }
        } else
        {
            light.intensity = 0f;
        }
    }


    public void SetColor(Color color)
    {
        light.color = color;
    }

    public Color GetColor()
    {
        return light.color;
    }

    public void ResetColor()
    {
        light.color = Color.white;
    }

    public void SetIntensity(float value)
    {
        if(searchForMultipleLights)
        {
            foreach(Light l in lights)
            {
                l.intensity = value;
            }
        }
        else light.intensity = value;
    }

    public void SetMinIntensity(float value)
    {
        minIntensity = value;
    }

    public float GetMinIntensity()
    {
        return minIntensity;
    }

    public void SetSmoothing(int value)
    {
        smoothing = value;
    }

    public float GetMaxIntensity()
    {
        float temp = 0;
        if (searchForMultipleLights)
        {
            foreach (Light l in lights)
            {
                if (temp < l.intensity) temp = l.intensity;
            }
        }
        else temp = light.intensity;

        return temp;
    }
}
