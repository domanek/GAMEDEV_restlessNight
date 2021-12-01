using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement movementScript;
    private LightFlicker flickerScript;

    [Header("Player Light Parameters")]
    [SerializeField] private Light playerLight;
    [SerializeField] private float minLightIntesity = 0f;
    [SerializeField] private float maxLightIntensity = 2f;
    [SerializeField] private float currentLightIntensity;
    [SerializeField] private float startLightIntensity = 1f;
    private float lastPlayerLightIntensity;

    [Header("Player Advanced Light Parameters")]
    [SerializeField] private float lightFallOff = 6;

    private bool flashlightIsOn = false;
    private bool flashlightIsDecreasing = false;
    private bool flashLightIsInfluenced = false;

    private bool flashlightIsFlickering = false;
    private int flickCounter = 0;
    private float flickerStartIntensity;

    [Header("Player Standup Parameters")]
    [SerializeField] private float standUpSpeed = 4;

    [Header("Player Rain obj")]
    [SerializeField] private GameObject rainFieldObject;

    void Start()
    {
        playerLight = GetComponentInChildren<Light>();
        playerLight.intensity = startLightIntensity;
        lastPlayerLightIntensity = startLightIntensity;
        movementScript = GetComponent<PlayerMovement>();
        if (!flashlightIsOn) playerLight.intensity = 0;

        flickerScript = GetComponentInChildren<LightFlicker>();
        flickerScript.allowFlicker = false;
        flickerStartIntensity = flickerScript.GetMinIntensity();

        StartCoroutine(PlayStandUpSequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlightIsOn = !flashlightIsOn;
            if (!flashlightIsOn)
            {
                AudioManager.instance.PlayWithPitch("flashlight_off", Random.Range(0.9f, 1.1f));
                lastPlayerLightIntensity = playerLight.intensity;
                playerLight.intensity = 0f;
            }
            else
            {
                AudioManager.instance.PlayWithPitch("flashlight_on", Random.Range(0.9f, 1.1f));
                playerLight.intensity = lastPlayerLightIntensity;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0.0f;
            HUD.instance.OpenPause();
        }
        ProcessInteraction();
        ProcessLight();
        ProcessLightFlicker();

        currentLightIntensity = playerLight.intensity;
    }

    public void ChangeLightInfluence()
    {
        flashLightIsInfluenced = true;
    }

    public void StartFlickerSequence()
    {
        ProcessLightFlicker(true);
    }

    private void ProcessLight()
    {
        if (flashlightIsOn && !flashlightIsDecreasing && flashLightIsInfluenced) StartCoroutine(DecreasePlayerLightIntensity());
    }

    private bool isResetingFlickCounter = false;
    private void ProcessLightFlicker(bool overrideCondition = false)
    {
        if ((flashlightIsOn && playerLight.intensity < 0.4f && playerLight.intensity > 0.25f) || overrideCondition)
        {
            if (!flashlightIsFlickering && flickCounter <= 5) StartCoroutine(FlickerPlayerLight());
        }

        if (!flashlightIsFlickering && !isResetingFlickCounter) StartCoroutine(ResetFlickCounter());
    }

    private IEnumerator ResetFlickCounter()
    {
        isResetingFlickCounter = true;
        yield return new WaitForSeconds(1.2f);
        if (flashlightIsFlickering) yield break;
        flickCounter = 0;
        yield return null;
        isResetingFlickCounter = false;
    }

    private IEnumerator FlickerPlayerLight()
    {
        flashlightIsFlickering = true;
        flickCounter++;

        flickerScript.SetMinIntensity(flickerStartIntensity);

        float temp = playerLight.intensity;
        flickerScript.SetSmoothing(Random.Range(0, 2));
        flickerScript.allowFlicker = true;

        //AudioManager.instance.Play("flicker");
        yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
        flickerScript.allowFlicker = false;
        flickerScript.SetIntensity(temp);

        flashlightIsFlickering = false;
    }

    private IEnumerator DecreasePlayerLightIntensity()
    {
        flashlightIsDecreasing = true;
        yield return new WaitForSeconds(1.0f);
        if (flashlightIsOn)
        {
            while (flashlightIsOn)
            {
                playerLight.intensity -= Time.deltaTime / lightFallOff;
                playerLight.intensity = Mathf.Clamp(playerLight.intensity, minLightIntesity, maxLightIntensity);
                yield return null;
            }
        }
        else
        {
            flashlightIsDecreasing = false;
            yield break;
        }
        flashlightIsDecreasing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectable"))
        {
            IncreaseLightIntensity(1f);
            Destroy(other.gameObject);
        }
    }

    [Header("Player Interaction")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] LayerMask raycastLayerMask;
    float raycastDistance = 6.0f;
    RaycastHit currentHit;
    bool touchingObject = false, touchedObject = false;

    private void ProcessInteraction()
    {
        touchingObject = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out currentHit, raycastDistance, raycastLayerMask);
        if (touchingObject && Input.GetKeyDown(KeyCode.E))
            currentHit.transform.gameObject.GetComponent<DoorButton>().Interact(this as PlayerInput);

        if (touchingObject && !touchedObject)
        {
            touchedObject = true;
        }
        if (!touchingObject && touchedObject)
        {
            //PlayerStats.hud.HideInteractHint();
            touchedObject = false;
        }
    }

    public void IncreaseLightIntensity(float value)
    {
        if (flashlightIsOn) playerLight.intensity += value;
        else lastPlayerLightIntensity += value;
    }

    public void DecreaseLightIntensity(float value)
    {
        if (flashlightIsOn) playerLight.intensity -= value;
        else lastPlayerLightIntensity -= value;
    }

    public float GetCurrentPlayerLightIntensity()
    {
        if (flashlightIsOn) return currentLightIntensity;
        else return lastPlayerLightIntensity;
    }
    public float GetMaxLightIntensity()
    {
        return maxLightIntensity;
    }

    public bool IsAbleToMove()
    {
        return movementScript.canMove;
    }

    public void StopMoving()
    {
        movementScript.canMove = false;
    }

    public bool GetFlashlightStatus()
    {
        if (playerLight.intensity <= 0.05f) return false;
        else return flashlightIsOn;
    }

    private IEnumerator PlayStandUpSequence()
    {
        TransitionManager.instance.StartTransition("wakeup");
        AudioManager.instance.Play("player_nightmare");

        LeanTween.rotateZ(this.gameObject, 0, standUpSpeed).setEaseInSine();
        yield return new WaitForSeconds(standUpSpeed);
        movementScript.canMove = true;

        TextManager.instance.StartTextEvent("wakeup");
        AudioManager.instance.Play("player_wakeup");
    }

    public void ToggleRainField()
    {
        rainFieldObject.SetActive(true);
    }
}
