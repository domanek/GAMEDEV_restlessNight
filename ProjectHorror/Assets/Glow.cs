using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    [SerializeField] Color targetColor;
    [SerializeField] Color startColor;

    private MeshRenderer rend;
    private Light light;

    private float effectTimer;
    [SerializeField] private float intensity = 0.2f;
    [SerializeField] float glowDuration = 2f;

    [SerializeField] private bool hasLight = false;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.material.SetColor("_EmissionColor", startColor);
        rend.material.EnableKeyword("_EMISSION");

        if (hasLight) light = GetComponent<Light>();
    }

    private void Update()
    {
        effectTimer = Mathf.PingPong(Time.time / glowDuration, 1f);
        rend.material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, effectTimer) * intensity);

        if (hasLight) light.intensity = effectTimer;
    }
}
