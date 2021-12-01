using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = false;

    [Header("Player Movement Sounds")]
    [SerializeField] private float velocityWalkThreshold = 0.1f;
    private float delay = 0;
    private float minPitch = 0.8f;
    private float maxPitch = 1.2f;
    [SerializeField] private float stepLengthWalk = 0.1f;
    [SerializeField] private float stepLengthRun = 0.3f;


    [Header("Player General Movement Parameters")]
    [SerializeField] private CharacterController contrl;
    [SerializeField] private float currentSpeed = 12f;
    [SerializeField] private float originalSpeed = 12f;
    [SerializeField] private float jumpHeight = 10;
    [SerializeField] private float gravity = -9.81f;
    Vector3 downwardVelocity;

    [Header("Player Ground Check Parameters")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask dirtGroundMask;
    [SerializeField] private LayerMask streetGroundMask;
    [SerializeField] private bool isOnDirt;
    [SerializeField] private bool isOnStreet;
    [SerializeField] private bool isFallingToFast = false;

    private bool allowSprint = true;
    [Header("Player Sprint Parameters")]
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float sprintTimer = 0f;
    [SerializeField] private float sprintDuration = 2f;

    public float totalDistance = 0;
    public bool trackDistance = true;
    private Vector3 prevLocation;

    void Start()
    {
        contrl = GetComponent<CharacterController>();
        originalSpeed = currentSpeed;

        prevLocation = transform.position;
    }

    private void FixedUpdate()
    {
        if (!(isOnDirt || isOnStreet)) RecordDistance();
        else
        {
            prevLocation = transform.position;
            totalDistance = 0;
        }
    }

    void RecordDistance()
    {
        Vector3 currentY = new Vector3(prevLocation.x, transform.position.y, prevLocation.z);
        totalDistance += Vector3.Distance(currentY, prevLocation);
        if (totalDistance > 300f)
        {
            AudioManager.instance.Play("falling");
            isFallingToFast = true;
        }
    }

    void Update()
    {
        if (!canMove) return;


        if (Input.GetKey(KeyCode.LeftShift) && allowSprint)
        {
            sprintTimer += Time.deltaTime;
            if (sprintTimer < sprintDuration) currentSpeed = sprintSpeed;
            else
            {
                FindObjectOfType<AudioManager>().Play("player_nobreath");
                allowSprint = false;
            }
        } else
        {
            sprintTimer -= Time.deltaTime;
            currentSpeed = originalSpeed;
        }
        sprintTimer = Mathf.Clamp(sprintTimer, 0, sprintDuration);
        if (sprintTimer == 0f) allowSprint = true;


        isOnDirt = Physics.CheckSphere(groundCheck.position, groundDistance, dirtGroundMask);
        isOnStreet = Physics.CheckSphere(groundCheck.position, groundDistance, streetGroundMask);
        if (isOnStreet) groundDistance = 1.35f;
        if (isOnDirt) groundDistance = 0.75f;

        if ((isOnDirt || isOnStreet) && downwardVelocity.y < 0)
        {
            downwardVelocity.y = -2f;
        }

        if (isFallingToFast && (isOnDirt || isOnStreet)) {
            LeanTween.rotateZ(this.gameObject, 90, 1).setEaseInSine();
            AudioManager.instance.StopPlay("falling");
            AudioManager.instance.Play("neck");
            isFallingToFast = false;
            GameController.instance.PlayerDie();    
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (isOnDirt || isOnStreet) PlayStepSound();
        if (Input.GetButtonDown("Jump") && (isOnDirt || isOnStreet)) downwardVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        downwardVelocity.y += gravity * Time.deltaTime;
        contrl.Move(downwardVelocity * Time.deltaTime);

        contrl.Move(move * currentSpeed * Time.deltaTime);
    }


    void PlayStepSound()
    {
        if (contrl.velocity.sqrMagnitude > velocityWalkThreshold)
        {
            if (currentSpeed == sprintSpeed)
            {
                if (delay >= stepLengthRun)
                {
                    //AudioManager.instance.PlaySoundAt(footstepSound, transform.position, footstepVolume);
                    //audioSource.PlayOneShot(footstepSound);
                    if (isOnDirt) AudioManager.instance.PlayWithPitch("player_footstep_ground", Random.Range(minPitch, maxPitch));
                    else AudioManager.instance.PlayWithPitch("player_footstep_street", Random.Range(minPitch, maxPitch));

                    delay = 0;
                }
                delay += Time.deltaTime;
            }
            else
            {
                if (delay >= stepLengthWalk)
                {
                    //AudioManager.instance.PlaySoundAt(footstepSound, transform.position, footstepVolume);
                    //audioSource.PlayOneShot(footstepSound);
                    if (isOnDirt) AudioManager.instance.PlayWithPitch("player_footstep_ground", Random.Range(minPitch, maxPitch));
                    else AudioManager.instance.PlayWithPitch("player_footstep_street", Random.Range(minPitch, maxPitch));
                    delay = 0;
                }
                delay += Time.deltaTime;
            }
        }
    }
}
