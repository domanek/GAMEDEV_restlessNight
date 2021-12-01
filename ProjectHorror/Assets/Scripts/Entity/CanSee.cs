using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSee : MonoBehaviour
{
    [SerializeField] private GameObject deathEffectPrefab;

    [SerializeField] private GameObject target;
    [SerializeField] private float lookDistance;
    [SerializeField] private float chaseThresold;
    [SerializeField] private float offsetUp = 3;
    RaycastHit hit;
    Vector3 rayDirection;
    private float targetHitDistance = 8f;

    public float currentPlayerDistance;
    public bool raycastHitPlayer;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    public bool CheckSeeStatus()
    {
        return raycastHitPlayer;
    }

    public float GetPlayerDistance()
    {
        return currentPlayerDistance;
    }


    void Update()
    {
        rayDirection = target.transform.position - transform.position;

        raycastHitPlayer = CheckRayCastHit(rayDirection);

        if (currentPlayerDistance < targetHitDistance)
        {
            if (target.gameObject.CompareTag("Player"))
            {
                AudioManager.instance.PlayWithPitch("enemy_hit", Random.Range(1.3f, 2f));
                var effect = Instantiate(deathEffectPrefab, transform.position, transform.rotation);
                Destroy(effect.gameObject, 1f);
                target.GetComponent<PlayerInput>().DecreaseLightIntensity(1f);
            }
            Destroy(gameObject, 0.015f);
        }

       //Debug.DrawRay(transform.position + Vector3.up * offsetUp, target.transform.position -  transform.position, Color.red);
       //Debug.DrawRay(transform.position + Vector3.up * offsetUp, rayDirection, Color.yellow);
       //Debug.DrawRay(transform.position, transform.forward * lookDistance, Color.green);
    }

    private bool CheckRayCastHit(Vector3 rayDirection)
    {
        if (Physics.Raycast(transform.position + Vector3.up * offsetUp, rayDirection, out hit))
        {
            if (rayDirection.magnitude < lookDistance) raycastHitPlayer = hit.collider.CompareTag("Player");
            else raycastHitPlayer = false;

            currentPlayerDistance = rayDirection.magnitude;
        }
        return raycastHitPlayer;
    }
}
