using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //CameraShake: Cam shaker script,  adjustable values
    public float shake = 0;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.5f;

    void Update()
    {
        if (shake > 0)
        {
            this.transform.localPosition = Random.insideUnitSphere * shakeAmount;
            shake -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shake = 0;
        }
    }

    public void Shake(float value)
    {
        shake = value;
    }
    public void SetTinyShake()
    {
        shakeAmount = 0.01f;
    }


    public void SetSmallShake()
    {
        shakeAmount = 0.03f;
    }

    public void SetMiddleShake()
    {
        shakeAmount = 0.07f;
    }

    public void SetStrongShake()
    {
        shakeAmount = 0.13f;
    }

    public void SetNoShake()
    {
        shakeAmount = 0f;
    }
}
