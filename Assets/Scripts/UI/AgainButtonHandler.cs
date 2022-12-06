using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgainButtonHandler : MonoBehaviour
{
    public void Click()
    {
        Camera.main.GetComponent<PauseManager>().UnPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
