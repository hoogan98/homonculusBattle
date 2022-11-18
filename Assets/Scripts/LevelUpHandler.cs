using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpHandler : MonoBehaviour
{
    public Utilities.LevelType levelType;
    public int points;
    public void LevelUp() {
        if (PlayerPrefs.GetInt(levelType.ToString(), 0) < points) {
            PlayerPrefs.SetInt(levelType.ToString(), points);
        }
    }
}
