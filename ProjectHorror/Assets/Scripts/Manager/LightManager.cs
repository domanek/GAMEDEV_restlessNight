using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private MyLight[] mainLights;

    [Header("LightManager Control Parameters")]
    [SerializeField] private float lightSwitchThreshold = 1f;
    [SerializeField] private float lightMaxIntensity = 1f;
    [SerializeField] private float lightMinIntensity = 0.05f;

    [SerializeField] private float lightChangeSpeed = 10f;

    private PlayerInput player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (player.GetCurrentPlayerLightIntensity() < lightSwitchThreshold)
        {
            foreach (MyLight l in mainLights)
            {
                if (l.light != null)
                {
                    l.light.intensity -= lightChangeSpeed * Time.deltaTime;
                    l.light.intensity = Mathf.Clamp(l.light.intensity, lightMinIntensity, lightMaxIntensity);


                }
            }

        } else {
            foreach (MyLight l in mainLights)
            {
                if (l.light != null)
                {

                    l.light.intensity += lightChangeSpeed * Time.deltaTime;
                    l.light.intensity = Mathf.Clamp(l.light.intensity, lightMinIntensity, lightMaxIntensity);
                }
            }
        }
    }
}

[System.Serializable]
public class MyLight
{
    public string name;

    public Light light;

    public float maxIntensity;
}
