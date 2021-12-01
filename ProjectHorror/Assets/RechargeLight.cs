using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeLight : MonoBehaviour
{
    [SerializeField] private float rechargeAmount = 0.05f;
    [SerializeField] private float rechargeWait = 0.1f;

    private LightFlicker flickerScript;

    private float effectTimer;
    [SerializeField] float glowDuration = 2f;
    [SerializeField] float offset = 0.1f;
    private bool isRecharging = false;

    private void Start()
    {
        flickerScript = GetComponent<LightFlicker>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isRecharging && !flickerScript.allowFlicker && flickerScript.GetMaxIntensity() != 0) StartCoroutine(RechargePlayerLight(other));
        }
    }

    private IEnumerator RechargePlayerLight(Collider other)
    {
        isRecharging = true;
        other.GetComponent<PlayerInput>().IncreaseLightIntensity(rechargeAmount);
        if (other.GetComponent<PlayerInput>().GetCurrentPlayerLightIntensity() < (other.GetComponent<PlayerInput>().GetMaxLightIntensity() - offset)) flickerScript.StartGlow(1);
        yield return new WaitForSeconds(rechargeWait);
        isRecharging = false;
    }
}
