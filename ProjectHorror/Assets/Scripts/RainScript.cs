using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainScript : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.Play("rain");
    }
}
