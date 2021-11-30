using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRain : MonoBehaviour
{
    private bool hasBeenActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !hasBeenActivated) 
        {
            other.GetComponent<PlayerInput>().ToggleRainField();

            hasBeenActivated = true;
        }
    }
}
