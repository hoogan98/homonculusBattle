using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnBrainDeath : MonoBehaviour
{
    private Brain attachedBrain;

    void Start() {
        attachedBrain = GetComponentInChildren<Brain>();
        Homonculus self = GetComponent<Homonculus>();

        if (self.enemy) {
            attachedBrain.OnDeath += DisplayWinText;
        } else {
            attachedBrain.OnDeath += ResetScene;
        }
    }

    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DisplayWinText() {
        Debug.Log("wiener");
    }
}
