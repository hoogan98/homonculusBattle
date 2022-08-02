using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    public string scene;
    private Button startButton;

    void Start()
    {
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(() => OnButtonHit());
    }

    void OnButtonHit()
    {
        SceneManager.LoadScene(scene);
    }
}
