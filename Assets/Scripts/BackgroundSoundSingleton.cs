using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSoundSingleton : MonoBehaviour
{
    public static BackgroundSoundSingleton instance;

    public AudioSource AudioSourceComponent => GetComponent<AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        AudioSourceComponent.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
    }
}
