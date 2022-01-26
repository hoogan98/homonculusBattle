using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerButtonHandler : MonoBehaviour
{
    public GameObject p1DrawZone;
    public GameObject p2DrawZone;
    public GameObject barriers;
    public Camera gameCam;
    public Text p1DrawText;
    public Text p2DrawText;
    public float gravity;

    private Button startButton;

    private bool p1Done;
    private bool ready;
    private bool startTime;
    private float time;

    void Start()
    {
        startButton = this.GetComponent<Button>();
        startButton.onClick.AddListener(() => OnButtonHit());

        p1Done = false;
        startTime = false;
        this.ready = false;

        this.time = 0;
    }

    void OnButtonHit()
    {
        if (!startTime) 
        {
            this.p1DrawZone.GetComponent<DrawParts>().isActive = true;

            this.startTime = true;
        }
        else if (!p1Done)
        {
            this.p1DrawZone.GetComponent<DrawParts>().isActive = false;

            this.p1Done = true;
        }
        else if (!this.ready)
        {
            this.p2DrawZone.GetComponent<DrawParts>().isActive = true;

            this.ready = true;
        }
        else
        {
            this.StartGame();
        }
    }

    void Update()
    {
        if (!this.startTime)
        {
            return;
        }

        if (!p1Done)
        {
            this.time += Time.deltaTime;
            this.startButton.GetComponentInChildren<Text>().text = this.time.ToString() + " seconds";
        }
        else if (!this.ready)
        {
            this.startButton.GetComponentInChildren<Text>().text = "Ready?";
        }
        else
        {
            this.time -= Time.deltaTime;
            this.startButton.GetComponentInChildren<Text>().text = this.time.ToString() + " seconds";
            if (this.time <= 0)
            {
                this.StartGame();
            }
        }
    }

    private void StartGame()
    {
        if (this.p1DrawZone == null || this.p2DrawZone == null)
        {
            return;
        }

        this.gameCam.GetComponent<WinCheck>().StartGame();

        List<GameObject> p1Parts = p1DrawZone.GetComponent<DrawParts>().GetParts();
        List<GameObject> p2Parts = p2DrawZone.GetComponent<DrawParts>().GetParts();

        foreach (GameObject part in p1Parts)
        {
            if (part == null)
            {
                continue;
            }
            if (!part.CompareTag("Muscle"))
            {
                part.GetComponent<Rigidbody2D>().gravityScale = gravity;
            }
            else
            {
                part.GetComponent<MuscleBehavior>().StartGame();
            }

            Destroy(part.GetComponent<DrawingBehavior>());
            part.GetComponent<BoxCollider2D>().isTrigger = false;
            part.GetComponent<DamageCheck>().StartGame();
        }
        foreach (GameObject part in p2Parts)
        {
            if (part == null)
            {
                continue;
            }
            if (!part.CompareTag("Muscle"))
            {
                part.GetComponent<Rigidbody2D>().gravityScale = gravity;
            }
            else
            {
                part.GetComponent<MuscleBehavior>().StartGame();
            }

            part.GetComponent<BoxCollider2D>().isTrigger = false;
            Destroy(part.GetComponent<DrawingBehavior>());
            part.GetComponent<DamageCheck>().StartGame();
        }

        Destroy(p1DrawZone);
        Destroy(p2DrawZone);
        Destroy(p1DrawText);
        Destroy(p2DrawText);
        Destroy(barriers);
        Destroy(this.gameObject);
    }
}
