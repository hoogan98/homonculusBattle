using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public string sceneName;
    public Utilities.LevelType levelType;
    public int unlocksRequired;

    public void Click() {
        Debug.Log(levelType.ToString());
        if (PlayerPrefs.GetInt(levelType.ToString(), 0) >= unlocksRequired) {
            SceneManager.LoadScene(sceneName);
        }
    }
}
