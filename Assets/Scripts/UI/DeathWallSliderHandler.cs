using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathWallSliderHandler : MonoBehaviour
{
    public Slider speedSlider;
    public float speedModifier;

    public void OnSliderChange()
    {
        Settings.instance.deathWallSpeed =speedSlider.value * speedModifier;
        Debug.Log(Settings.instance.deathWallSpeed);
    }
}
