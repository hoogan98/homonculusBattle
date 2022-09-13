using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;

    public void Start() {
        volumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
    }

    public void OnSliderChange()
    {
        PlayerPrefs.SetFloat("SFXVolume", volumeSlider.value);
        BackgroundSoundSingleton.instance.AudioSourceComponent.volume = volumeSlider.value;
    }
}
