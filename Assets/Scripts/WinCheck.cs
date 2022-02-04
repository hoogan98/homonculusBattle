using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCheck : MonoBehaviour
{
    public GameObject p1Brain;
    public GameObject p2Brain;
    public GameObject canvas;
    public Button menuButton;
    public Button againButton;
    public Text winText;

    private bool _started;

    private void Start()
    {
        _started = false;
    }

    void Update()
    {
        if (!_started)
        {
            return;
        }

        if (p1Brain == null)
        {
            winText.text = "P2 Wins";
            Destroy(this);
            Instantiate(againButton, canvas.transform);
            Instantiate(menuButton, canvas.transform);
        }
        if (p2Brain == null)
        {
            winText.text = "P1 Wins";
            Destroy(this);
            Instantiate(menuButton, canvas.transform);
            Instantiate(againButton, canvas.transform);
        }
    }

    public void AssignBrain(bool isp1, GameObject Brain)
    {
        if (isp1)
        {
            p1Brain = Brain;
        }
        else
        {
            p2Brain = Brain;
        }
    }

    public void StartGame()
    {
        _started = true;
    }
}
