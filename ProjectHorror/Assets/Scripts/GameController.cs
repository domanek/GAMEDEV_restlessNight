using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public PlayerInput player;

    private AudioSource buildUpTensionAudioSrc;
    private float maximumTensionVolume = 0.05f;
    private float tensionVolumeChangeSpeed = 0.01f;

    [Header("Player UI Components")]
    [SerializeField] private Slider playerLightIntensitySlider;
    [SerializeField] private Image deathImage;

    [Header("Fog Control")]
    [SerializeField] private float fogThreshold = 0.25f;
    [SerializeField] private float fogChangeSpeed = 25;
    [SerializeField] private float minFog = 25;
    [SerializeField] private float maxFog = 100;

    private float currentPlayerLightIntensity;

    private bool playerDied = false;
    private bool playedFogText = false;
    private bool isPlayingTensionSound = false;
    private bool isStopingTensionSound = false;

    void Start()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        AudioManager.instance.Play("theme");
        //SetupSlider();
        playerDied = false;

        buildUpTensionAudioSrc = GetComponent<AudioSource>();
        buildUpTensionAudioSrc.volume = 0;
    }


    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        }

        if (!player.IsAbleToMove()) return;

        currentPlayerLightIntensity = player.GetCurrentPlayerLightIntensity();


        if (currentPlayerLightIntensity < fogThreshold && !playerDied)
        {
            RenderSettings.fogEndDistance -= Time.deltaTime * fogChangeSpeed;

            if (!playedFogText)
            {
                playedFogText = true;
                TextManager.instance.StartTextEvent("fog");
            }

            if (!isPlayingTensionSound) StartCoroutine(PlayTensionSound());
        } else
        {
            RenderSettings.fogEndDistance += Time.deltaTime * fogChangeSpeed;
            if (!isPlayingTensionSound) StartCoroutine(StopTensionSound());
        }
        RenderSettings.fogEndDistance = Mathf.Clamp(RenderSettings.fogEndDistance, minFog, maxFog);

        if (player.GetCurrentPlayerLightIntensity() > 0.4f)
        {
            playedFogText = false;
        }
        //if (player.GetCurrentPlayerLightIntensity() <= minimumLightIntensity && !playerDied)
        //{
        //    AudioManager.instance.PlayWithPitch("player_death", Random.Range(1.2f, 1.5f)); // DOES NOT PLAY?!?!?!? cant hear
        //    TransitionManager.instance.StartTransition("death");
        //
        //    PlayerDie();
        //}


        //playerLightIntensitySlider.value = player.GetCurrentPlayerLightIntensity();
        CheckAntiStuck();
    }

    private IEnumerator PlayTensionSound()
    {
        isPlayingTensionSound = true;

        while (buildUpTensionAudioSrc.volume < maximumTensionVolume)
        {
            buildUpTensionAudioSrc.volume += Time.deltaTime * tensionVolumeChangeSpeed;

            if (player.GetCurrentPlayerLightIntensity() > fogThreshold)
            {
                isPlayingTensionSound = false;
                yield break;
            }
            yield return null;
        }
        yield return null;

        isPlayingTensionSound = false;
    }

    private IEnumerator StopTensionSound()
    {
        isStopingTensionSound = true;

        while (buildUpTensionAudioSrc.volume > 0)
        {
            buildUpTensionAudioSrc.volume -= Time.deltaTime * tensionVolumeChangeSpeed * 10;

            if (isPlayingTensionSound)
            {
                isStopingTensionSound = false;
                break;
            }

            yield return null;
        }
        yield return null;

        isStopingTensionSound = false;
    }

private void CheckAntiStuck()
    {
        if (player.transform.position.y < -10)
        {
            PlayerDie();
        }
    }

    public void PlayerDie()
    {
        playerDied = true;
        TransitionManager.instance.StartTransition("death");
        player.StopMoving();
        Invoke("RestartScene", 1f);
    }

    private void RestartScene()
    {
        AudioManager.instance.StopPlayAll();
        TextManager.instance.StopAllTextEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetupSlider()
    {
        playerLightIntensitySlider.maxValue = player.GetMaxLightIntensity();
    }
}
