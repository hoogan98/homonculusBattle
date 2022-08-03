using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSliderHandler : MonoBehaviour
{
    public Slider brightnessSlider;

    public void Start() {
        brightnessSlider.value = Settings.instance.brightness;
    }

    public void OnSliderChange()
    {
        Settings.instance.brightness = brightnessSlider.value;
    }
}
