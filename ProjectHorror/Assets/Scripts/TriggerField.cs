using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerField : MonoBehaviour
{
    [Header("Can Be triggered multiple times?")]
    [SerializeField] private bool canBeTriggeredMutlipleTimes = false;

    [Header("TriggerField parameters")]
    [SerializeField] private string triggerName;
    [SerializeField] private GameObject connectedObject;
    [SerializeField] private GameObject connectedObjectSpawn = null;
    [SerializeField] private GameObject connectedLight;

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!canBeTriggeredMutlipleTimes && !hasBeenTriggered)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (connectedObject != null) TriggerManager.instance.StartTriggerEvent(triggerName, connectedObject, connectedObjectSpawn, connectedLight);
                else TriggerManager.instance.StartTriggerEvent(triggerName);

                hasBeenTriggered = true;
            }
        } 
        else if (canBeTriggeredMutlipleTimes && !hasBeenTriggered)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (connectedObject != null) TriggerManager.instance.StartTriggerEvent(triggerName, connectedObject, connectedObjectSpawn, connectedLight);
                else TriggerManager.instance.StartTriggerEvent(triggerName);

                hasBeenTriggered = true;
            }
        }
    }
}
