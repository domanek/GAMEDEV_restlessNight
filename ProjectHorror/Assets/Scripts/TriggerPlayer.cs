using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayer : MonoBehaviour
{
    private bool triggerOnce = false;
    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce) return;
        triggerOnce = true;

        if (other.gameObject.CompareTag("Player"))
        {
        other.GetComponent<PlayerInput>().ChangeLightInfluence();

        }
    }
}
