using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnBrainDeath : MonoBehaviour
{
    void Start() {
        GetComponent<Homonculus>().OnBrainDeath += ResetScene;
    }

    public void ResetScene() {
        Debug.Log("running scene reset");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
