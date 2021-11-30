using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy behaviour")]
    [SerializeField] private bool isReactive = true;
    [SerializeField] private bool isAffectedByLight = true;
    [SerializeField] private float fleeingDuration = 2f;
    [SerializeField] private float chasingDuration = 1f;
    [SerializeField] private bool hasMovingInstruction = false;
    [SerializeField] private GameObject moveDestination;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;

    private Rigidbody rb;
    private LightVulnerability lightVulnerabilityScript;
    private AnimationStateController animationScript;
    private CanSee canSeeScript;

    [Header("Enemy movement parameters")]
    [SerializeField] private float chaseDuration;
    [SerializeField] private float chaseSpeed;

    [SerializeField] private float fleeingSpeed;

    [Header("Enemy audio parameters")]
    [SerializeField] private float timer;
    [SerializeField] private float audioIdleDuration;
    [SerializeField] private float audioChaseDuration;
    [SerializeField] private float audioFleeingDuration;

    private GameObject player;

    private bool isPlayingAudio = false;
    private bool isBeeingSpotted = false;
    private bool isApproaching = false;
    private bool isEscaping = false;
    private bool isGoingToPosition = false;

    [SerializeField] private EnemyState currentState = EnemyState.Idle;

    private Renderer rend;

    enum EnemyState
    {
        Idle,
        Chasing,
        Fleeing
    }

    private bool flashing = false;
    public void EnemyHitEffect()
    {
        if (!flashing && lightVulnerabilityScript.GetLightStatus() && isAffectedByLight) StartCoroutine(FlashObject(Color.black, Color.white, 1f, 0.15f));
    }

    void ResetColor()
    {
        rend.material.DisableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", Color.black);
        rend.material.color = Color.black;
    }

    private IEnumerator FlashObject(Color originalColor, Color flashColor, float flashTime, float flashSpeed)
    {
        flashing = true;

        rend.material.EnableKeyword("_EMISSION");
        float flashingFor = 0;
        Color newColor = flashColor;
        while (flashingFor < flashTime)
        {
            if (lightVulnerabilityScript.GetLightTimer() > 1.5f)
            {
                Debug.Log("Flashing faster");
                flashSpeed /= 2;
            }
            flashingFor += flashSpeed;

            if (newColor == flashColor)
            {
                rend.material.SetColor("_EmissionColor", originalColor);
                rend.material.color = originalColor;
                newColor = originalColor;
            }
            else
            {
                rend.material.SetColor("_EmissionColor", flashColor);
                rend.material.color = flashColor;
                newColor = flashColor;
            }
            yield return new WaitForSeconds(flashSpeed);
        }

        yield return null;
        rend.material.DisableKeyword("_EMISSION");

        flashing = false;
    }

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
        lightVulnerabilityScript = GetComponent<LightVulnerability>();
        animationScript = GetComponent<AnimationStateController>();
        canSeeScript = GetComponent<CanSee>();
        player = GameObject.FindGameObjectWithTag("Player");

        chaseDuration = Random.Range(chaseDuration, chaseDuration * 3);
        chaseSpeed = Random.Range(chaseSpeed, chaseSpeed * 2);
        fleeingSpeed = Random.Range(chaseSpeed, fleeingSpeed + fleeingSpeed / 4);

        if (hasMovingInstruction) animationScript.SetAnimRunning(true); 
    }

    private void FixedUpdate()
    {
        if (player == null || !isReactive) return;

        if (hasMovingInstruction)
        {
            if (!isGoingToPosition) StartCoroutine(GoToPos());
        } else
        {
            GetEnemyLightStatus();
            SetEnemyBehaviour();
        }
    }

    private IEnumerator GoToPos()
    {
        isGoingToPosition = true;

        Vector3 lookPos;
        if (moveDestination == null)
        {
            Vector3 temp = transform.position + moveDirection;
            lookPos = temp - transform.position;
            Debug.Log("MoveDest was null! changed with " + moveDirection);
        } else
        {
            lookPos = moveDestination.transform.position - transform.position;
        }

        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);

        bool notRotated = true;
        float temp2 = 0;
        while (notRotated)
        {
            if (temp2 < 0.5f) notRotated = false;
            temp2 += Time.deltaTime * 5;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, temp2);
            yield return null;
        }
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, );
        transform.position += transform.forward * Time.deltaTime * chaseSpeed * 2;
        yield return new WaitForSeconds(chaseDuration);
        if (lookPos.magnitude < 2f) hasMovingInstruction = false;

        isGoingToPosition = false;
    }

    private void GetEnemyLightStatus()
    {
        float currentLightValue = lightVulnerabilityScript.GetLightTimer();

        if (false)
        {
            currentState = EnemyState.Fleeing;
            //AudioManager.instance.Play("enemy_light");
        }
        else if (currentLightValue > 0.1f && currentLightValue <= 1.55f || canSeeScript.CheckSeeStatus())
        {
            currentState = EnemyState.Chasing;
            if (canSeeScript.GetPlayerDistance() < 40) AudioManager.instance.PlayShortAtPosition("enemy_chasing", 2, this.transform.position);
            EnemyHitEffect();
        } else
        {
            currentState = EnemyState.Idle;
            if (canSeeScript.GetPlayerDistance() < 50) AudioManager.instance.PlayShortAtPosition("enemy_idle", 2, this.transform.position);
        }
        SetAnimationState();
    }

    private void SetEnemyBehaviour()
    {
        if (currentState == EnemyState.Chasing)
        {
            //StartAudio();
            StartChasing();
        }
        else if (currentState == EnemyState.Fleeing)
        {
            //StartAudio();
            StartFleeing();
        }
        else
        {
            //StartAudio();
        }
    }

    private void SetAnimationState()
    {
        if (currentState == EnemyState.Idle)
        {
            animationScript.SetAnimRunning(false);
            animationScript.SetAnimFleeing(false);
        }
        else if (currentState == EnemyState.Chasing)
        {
            animationScript.SetAnimRunning(true);
        }
        else if (currentState == EnemyState.Fleeing)
        {
            animationScript.SetAnimRunning(true);
        }
    }

    public void HasBeenSpotted()
    {   
        if (!isBeeingSpotted) StartCoroutine(EnemyHasBeenSpotted());
    }

    private IEnumerator EnemyHasBeenSpotted()
    {
        isBeeingSpotted = true;

        lightVulnerabilityScript.SetLightStatus(true);
        EnemyHitEffect();

        yield return new WaitForSeconds(0.15f);

        lightVulnerabilityScript.SetLightStatus(false);

        isBeeingSpotted = false;
    }

    private void StartChasing()
    {
        if (!isApproaching) StartCoroutine(ChasePlayer(chaseDuration));
    }

    private void StartFleeing()
    {
        if (!isEscaping) StartCoroutine(FleeingPlayer(chaseDuration));
    }

    private IEnumerator ChasePlayer(float approachDuration)
    {
        isApproaching = true;

        MoveToTarget();
        yield return new WaitForSeconds(approachDuration);

        isApproaching = false;
    }

    private IEnumerator FleeingPlayer(float approachDuration)
    {
        isEscaping = true;

        MoveAwayFromTarget();
        yield return new WaitForSeconds(approachDuration);

        isEscaping = false;
    }

    private void MoveToTarget()
    {
        Debug.Log("MoveToTarget");
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);

        //transform.LookAt(player.transform);
        //transform.LeanMove(transform.position * transform.forward * Time.deltaTime, approachDuration).setEaseInSine();
        transform.position += transform.forward * Time.deltaTime * chaseSpeed;
    }

    private void MoveAwayFromTarget()
    {
        var lookPos = transform.position - player.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);

        //transform.LookAt(player.transform);
        //transform.LeanMove(transform.position * transform.forward * Time.deltaTime, approachDuration).setEaseInSine();
        transform.position += transform.forward * Time.deltaTime * fleeingSpeed;
    }

    private void StartAudio()
    {
        if (!isPlayingAudio) StartCoroutine(AudioSequence());
    }

    private IEnumerator AudioSequence(string audioName = "peter", float waitingFor = 2)
    {
        isPlayingAudio = true;

        if (currentState == EnemyState.Idle) AudioManager.instance.PlayShortAtPosition("enemy_idle", audioIdleDuration, transform.position);
        else if (currentState == EnemyState.Chasing) AudioManager.instance.PlayShortAtPosition("enemy_chasing", audioChaseDuration, transform.position);
        else if (currentState == EnemyState.Fleeing) AudioManager.instance.PlayShortAtPosition("enemy_fleeing", audioFleeingDuration, transform.position);

        yield return new WaitForSeconds(waitingFor);

        if (currentState == EnemyState.Idle) AudioManager.instance.StopPlay("enemy_idle");
        else if (currentState == EnemyState.Chasing) AudioManager.instance.StopPlay("enemy_chasing");
        else if (currentState == EnemyState.Fleeing) AudioManager.instance.StopPlay("enemy_fleeing");

        //float waitForAnotherAmount = 0;
        //if (currentState == EnemyState.Idle) waitForAnotherAmount = 5;
        //else if (currentState == EnemyState.Chasing) waitForAnotherAmount = 2;
        //else if (currentState == EnemyState.Fleeing) waitForAnotherAmount = 3;
        //Debug.Log("NowWaitingFor " + waitForAnotherAmount + "..");
        yield return new WaitForSeconds(1);

        isPlayingAudio = false;
    }

    //private float delay = 0;
    //private float minPitch = 0.8f;
    //private float maxPitch = 1.2f;
    //[SerializeField] private float stepLengthWalk = 0.1f;
    //[SerializeField] private float stepLengthRun = 0.3f;
    //void PlayStepSound()
    //{
    //    Debug.Log("Enemy velocity: " + rb.velocity);
    //    if (rb.velocity.sqrMagnitude > 0.1f)
    //    {
    //        if (delay >= stepLengthRun)
    //        {
    //            AudioManager.instance.PlayWithPitch("enemy_chasing", Random.Range(minPitch, maxPitch));
    //            delay = 0;
    //        }
    //        delay += Time.deltaTime;
    //    }
    //}

}
