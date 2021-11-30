using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [Header("Choose 0 / 1 ")]
    [SerializeField] private Vector3 axis;

    void Start()
    {
        transform.Rotate(axis * Random.Range(0,180));
    }
}
