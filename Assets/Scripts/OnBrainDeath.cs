using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnBrainDeath : MonoBehaviour
{
    public GameObject canvas;
    public GameObject menuButton;
    public GameObject againButton;
    public Text winText;
    public AudioClip winSound;

    private Brain attachedBrain;
    private WinPhraseGenerator winGenerator;
    private Homonculus self;

    void Start() {
        attachedBrain = GetComponentInChildren<Brain>();
        self = GetComponent<Homonculus>();

        try {
            winGenerator = GetComponent<WinPhraseGenerator>();
            canvas = GameObject.Find("Canvas");
            winText = GameObject.Find("Canvas/WinText").GetComponent<Text>();
        } catch {
            Debug.Log("win ui text error");
        }

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
        winText.text = "You are the " + winGenerator.GetTitle();
        canvas.GetComponent<AudioSource>().PlayOneShot(winSound);
        
        Instantiate(againButton, canvas.transform);
        Instantiate(menuButton, canvas.transform);
        Destroy(this);
        return;
    }
}
