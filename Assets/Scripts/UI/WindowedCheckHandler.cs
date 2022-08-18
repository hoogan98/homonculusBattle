using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowedCheckHandler : MonoBehaviour
{
    public Toggle checkbox;

    public void Start() {
        checkbox.isOn = (PlayerPrefs.GetInt("Windowed", 0) == 1);
    }

    public void OnToggle() {
        PlayerPrefs.SetInt("Windowed", checkbox.isOn ? 1:0);
        Screen.fullScreen = checkbox.isOn;
    }
}
