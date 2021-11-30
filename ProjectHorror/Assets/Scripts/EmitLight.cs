using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitLight : MonoBehaviour
{
    [SerializeField] private Light lightComponent;
    [SerializeField] private float maxIntensity = 3f;

    private void Start()
    {
        lightComponent = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        lightComponent.intensity = Mathf.PingPong(Time.time, maxIntensity);
    }
}
