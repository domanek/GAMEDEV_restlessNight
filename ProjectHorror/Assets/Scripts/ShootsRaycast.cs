using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootsRaycast : MonoBehaviour
{
    private PlayerInput player;

    [Header("General Raycast Parameters")]
    [SerializeField] private string checkFor = "Enemy";
    [SerializeField] private float lookDistance = 25;
    [SerializeField] private float fieldOfView = 170;

    RaycastHit hit;
    Vector3 rayDirection;
    private bool raycastHitTarget;

    private void Start()
    {
        player = GetComponent<PlayerInput>();
    }

    private bool CheckRayCastHit(Vector3 rayDirection)
    {
        if (Physics.Raycast(transform.position, rayDirection, out hit))
        {
            raycastHitTarget = hit.collider.CompareTag(checkFor);
            if (raycastHitTarget && player.GetFlashlightStatus())
            {
                hit.collider.gameObject.GetComponent<Enemy>().HasBeenSpotted();
            }
        }
        return raycastHitTarget;
    }

    void Update()
    {
        rayDirection = Camera.main.transform.forward * lookDistance;
        Debug.DrawRay(Camera.main.transform.position, rayDirection, Color.cyan);
        raycastHitTarget = CheckRayCastHit(rayDirection);
    }
}
