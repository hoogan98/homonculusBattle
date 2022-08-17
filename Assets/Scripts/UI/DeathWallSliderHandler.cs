using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathWallSliderHandler : MonoBehaviour
{
    public Slider speedSlider;

    public void Start() {
        speedSlider.value = PlayerPrefs.GetFloat("DeathWallSpeed", 1);
    }

    public void OnSliderChange()
    {
        PlayerPrefs.SetFloat("DeathWallSpeed", speedSlider.value);
    }
}
