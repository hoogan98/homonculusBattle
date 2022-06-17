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
            DrawParts p1Drawer = p1DrawZone.GetComponent<DrawParts>();
            p1Drawer.isActive = true;
            p1Drawer.AttemptLoad();

            _startTime = true;
        }
        else if (!_p1Done)
        {
            p1DrawZone.GetComponent<DrawParts>().isActive = false;

            _p1Done = true;
        }
        else if (!_ready)
        {
            DrawParts p2Drawer = p2DrawZone.GetComponent<DrawParts>();
            p2Drawer.isActive = true;
            p2Drawer.AttemptLoad();

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

        DrawParts drawer1 = p1DrawZone.GetComponent<DrawParts>();
        DrawParts drawer2 = p2DrawZone.GetComponent<DrawParts>();

        drawer1.SavePlayer();
        drawer2.SavePlayer();

        List<GameObject> parts = drawer1.GetParts();
        parts.AddRange(drawer2.GetParts());

        foreach (GameObject part in parts)
        {
            if (part == null)
            {
                continue;
            }

            Part p = part.GetComponent<Part>();

            if (p == null)
            {
                continue;
            }

            p.LoadPart();
        }
        
        foreach (GameObject wall in movingDeathWalls)
        {
            wall.GetComponent<DeathWallMover>().enabled = true;
        }

        Camera.main.GetComponent<CamFollowDouble>().BeginGame(drawer1.GetPlayer().transform, drawer2.GetPlayer().transform);

        Destroy(p1DrawZone);
        Destroy(p2DrawZone);
        Destroy(p1DrawText);
        Destroy(p2DrawText);
        Destroy(barriers);
        Destroy(gameObject);
        
        Camera.main.GetComponent<WinCheck>().StartGame();
    }
}
