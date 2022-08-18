using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class BrightnessLoader : MonoBehaviour
{
    public PostProcessProfile brightnessFilter;

    public void Start()
    {
        brightnessFilter.GetSetting<ColorGrading>().postExposure.Override(PlayerPrefs.GetFloat("Brightness", 1));
    }
}
