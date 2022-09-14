using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class BrightnessSliderHandler : MonoBehaviour
{
    public Slider brightnessSlider;
    public PostProcessProfile brightnessFilter;

    public void Start() {
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1);
    }

    public void OnSliderChange()
    {
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
        brightnessFilter.GetSetting<ColorGrading>().postExposure.Override(brightnessSlider.value);
    }
}
