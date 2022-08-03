using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathWallSliderHandler : MonoBehaviour
{
    public Slider speedSlider;

    public void Start() {
        speedSlider.value = Settings.instance.deathWallSpeed;
    }

    public void OnSliderChange()
    {
        Settings.instance.deathWallSpeed = speedSlider.value;
    }
}
