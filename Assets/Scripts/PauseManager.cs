using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance => _instance;
    public bool IsPaused => Time.timeScale == 0;
    public GameObject pauseMenu;

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
            pauseMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }

    public void QuitToMainMenu()
    {
        try
        {
            SceneManager.MoveGameObjectToScene(GameObject.Find("win box"), SceneManager.GetActiveScene());
        }
        catch
        {
            Debug.Log("no winbox?");
        }
        SceneManager.LoadScene("Scenes/Menu");
    }
}
