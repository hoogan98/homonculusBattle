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
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnPause();
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

        UnPause();

        SceneManager.LoadScene("Scenes/Menu");
    }

    public void Pause() {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void UnPause() {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
