using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance => _instance;
    public bool IsPaused => Time.timeScale == 0;

    private static PauseManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsPaused)
        {
            Time.timeScale = 0;
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
        }
    }
}
