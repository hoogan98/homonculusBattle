using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;

    public void Start() {
        volumeSlider.value = Settings.instance.SFXVolume;
    }

    public void OnSliderChange()
    {
        Settings.instance.SFXVolume = volumeSlider.value;
    }
}
