using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    [SerializeField] Color targetColor;
    [SerializeField] Color startColor;

    Renderer rend;

    private float effectTimer;
    private float intensity = 0.2f;
    [SerializeField] float glowDuration = 2f;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.SetColor("_EmissionColor", startColor);
        rend.material.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        effectTimer = Mathf.PingPong(Time.time / glowDuration, 1f);
        rend.material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, effectTimer) * intensity);
    }
}
