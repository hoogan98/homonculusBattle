using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerButtonHandler : MonoBehaviour
{
    public GameObject p1DrawZone;
    public GameObject p2DrawZone;
    public GameObject barriers;
    public Text p1DrawText;
    public Text p2DrawText;
    public float gravity;
    public List<GameObject> movingDeathWalls;

    private Button _startButton;
    private bool _p1Done;
    private bool _ready;
    private bool _startTime;
    private float _time;

    void Start()
    {
        _startButton = GetComponent<Button>();
        _startButton.onClick.AddListener(() => OnButtonHit());

        _p1Done = false;
        _startTime = false;
        _ready = false; 
        _time = 0;
    }

    void OnButtonHit()
    {
        if (!_startTime) 
        { 
            p1DrawZone.GetComponent<DrawParts>().isActive = true;

            _startTime = true;
        }
        else if (!_p1Done)
        {
            p1DrawZone.GetComponent<DrawParts>().isActive = false;

            _p1Done = true;
        }
        else if (!_ready)
        {
            p2DrawZone.GetComponent<DrawParts>().isActive = true;

            _ready = true;
        }
        else
        {
            StartGame();
        }
    }

    void Update()
    {
        if (!_startTime)
        {
            return;
        }

        if (!_p1Done)
        {
            _time += Time.deltaTime;
            _startButton.GetComponentInChildren<Text>().text = _time + " seconds";
        }
        else if (!_ready)
        {
            _startButton.GetComponentInChildren<Text>().text = "Ready?";
        }
        else
        {
            _time -= Time.deltaTime;
            _startButton.GetComponentInChildren<Text>().text = _time + " seconds";
            if (_time <= 0)
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        if (p1DrawZone == null || p2DrawZone == null)
        {
            return;
        }

        Camera.main.GetComponent<WinCheck>().StartGame();

        List<GameObject> parts = p1DrawZone.GetComponent<DrawParts>().GetParts();
        parts.AddRange(p2DrawZone.GetComponent<DrawParts>().GetParts());

        foreach (GameObject part in parts)
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
                part.GetComponent<Muscle>().StartGame();
            }

            Destroy(part.GetComponent<DrawingBehavior>());
            part.GetComponent<BoxCollider2D>().isTrigger = false;
            part.GetComponent<Part>().StartGame();
        }
        
        foreach (GameObject wall in movingDeathWalls)
        {
            wall.GetComponent<DeathWallMover>().enabled = true;
        }

        Destroy(p1DrawZone);
        Destroy(p2DrawZone);
        Destroy(p1DrawText);
        Destroy(p2DrawText);
        Destroy(barriers);
        Destroy(gameObject);
    }
}
