using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    SettingsManager sm = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.startingVolume = s.volume;
        }

        sm = SettingsManager.instance;

        //DontDestroyOnLoad(this.gameObject);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " was not found!");
            return;
        }

        if (!s.source.isPlaying && this.gameObject != null) {
            //Debug.Log("Sound: " + name + " played!");

            if (s.name == "theme" || s.name == "theme2")
            {
                s.source.volume = s.startingVolume * sm.musicVolume;
                s.source.Play();
            } else
            {
                s.source.volume = s.startingVolume * sm.soundVolume;
                s.source.Play();
            }
        }
    }

    private bool shortSoundIsPlaying = false;
    public void PlayShort(string name, float duration)
    {
        if (!shortSoundIsPlaying) StartCoroutine(StartPlayShort(name, duration));
    }

    public IEnumerator StartPlayShort(string name, float duration, bool waitAfter = true)
    {
        shortSoundIsPlaying = true;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " was not found!");
            yield break;
        }

        if (!s.source.isPlaying)
        {
            //Debug.Log("Sound: " + name + " played!");
            s.source.volume = s.startingVolume * sm.soundVolume;
            s.source.Play();
        }
        yield return new WaitForSeconds(duration);
        s.source.Stop();
        yield return new WaitForSeconds(duration * 2);

        shortSoundIsPlaying = false;
    }

    public void PlayShortAtPosition(string name, float duration, Vector3 pos)
    {
        if (!shortSoundIsPlaying) StartCoroutine(StartPlayShortAtPos(name, duration, pos));
    }

    public IEnumerator StartPlayShortAtPos(string name, float duration, Vector3 pos)
    {
        shortSoundIsPlaying = true;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " was not found!");
            yield break;
        }

        GameObject testSound = new GameObject();
        if (!s.source.isPlaying || true)
        {
            //Debug.Log("PlayClipAtPoint: " + name + " played! At: " + pos);

            testSound.name = "gotestSound1";
            testSound.GetComponent<Transform>().position = pos;
            testSound.AddComponent<AudioSource>();
            testSound.GetComponent<AudioSource>().clip = s.clip;
            testSound.GetComponent<AudioSource>().volume = s.volume * sm.soundVolume;
            testSound.GetComponent<AudioSource>().pitch = s.pitch;
            testSound.GetComponent<AudioSource>().loop = s.loop;
            testSound.GetComponent<AudioSource>().playOnAwake = true;
            testSound.GetComponent<AudioSource>().spatialBlend = 1;
            testSound.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Custom;
            testSound.GetComponent<AudioSource>().maxDistance = 100;
            testSound.GetComponent<AudioSource>().Play();
        }
        yield return new WaitForSeconds(duration);

        Destroy(testSound);
        yield return new WaitForSeconds(duration * 2);

        shortSoundIsPlaying = false;
    }

    public void PlayWithPitch(string name, float pitchVal)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " was not found!");
            return;
        }

        if (!s.source.isPlaying)
        {
            //Debug.Log("Sound: " + name + " played!");
            s.source.pitch = pitchVal;
            s.source.volume = s.startingVolume * sm.soundVolume;
            s.source.Play();
        }
    }

    public void StopPlay(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " was not found!");
            return;
        }

        s.source.Stop();
    }

    public void StopPlayAll()
    {
        foreach(Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public void RestartAll()
    {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
                Play(s.name);
            }
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0, 1)]
    public float volume;
    [Range(0, 1.5f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
    [HideInInspector]
    public float startingVolume;
}

