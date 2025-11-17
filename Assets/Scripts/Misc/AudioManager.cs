using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    AudioSource audioPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            audioPlayer = GetComponent<AudioSource>();
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PlaySound(AudioClip audio, float volume)
    {
        audioPlayer.PlayOneShot(audio, volume);
    }
}
