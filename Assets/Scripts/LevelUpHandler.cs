using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpHandler : MonoBehaviour
{
    public string levelType;
    public int points;
    public void LevelUp() {
        if (PlayerPrefs.GetInt(levelType, 99) < points) {
            PlayerPrefs.SetInt(levelType, points);
        }
    }
}
