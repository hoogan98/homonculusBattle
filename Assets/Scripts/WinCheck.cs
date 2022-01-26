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

    private bool started;

    private void Start()
    {
        this.started = false;
    }

    void Update()
    {
        if (!this.started)
        {
            return;
        }

        if (this.p1Brain == null)
        {
            this.winText.text = "P2 Wins";
            Destroy(this);
            Instantiate(againButton, canvas.transform);
            Instantiate(menuButton, canvas.transform);
        }
        if (this.p2Brain == null)
        {
            this.winText.text = "P1 Wins";
            Destroy(this);
            Instantiate(menuButton, canvas.transform);
            Instantiate(againButton, canvas.transform);
        }
    }

    public void AssignBrain(bool isp1, GameObject Brain)
    {
        if (isp1)
        {
            this.p1Brain = Brain;
        }
        else
        {
            this.p2Brain = Brain;
        }
    }

    public void StartGame()
    {
        this.started = true;
    }
}
