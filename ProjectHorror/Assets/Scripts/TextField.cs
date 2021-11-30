using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextField : MonoBehaviour
{
    [SerializeField] private string textName;

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !hasBeenTriggered)
        {
            TextManager.instance.StartTextEvent(textName);

            hasBeenTriggered = true;
        }
    }
}
