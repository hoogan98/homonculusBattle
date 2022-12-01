using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMainMenu : MonoBehaviour
{
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

        Camera.main.GetComponent<PauseManager>().UnPause();

        SceneManager.LoadScene("Scenes/Menu");
    }
}
