using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObstacleModeStarter : MonoBehaviour
{
    public GameObject p1DrawZone;
    public Text p1DrawText;
    public float gravity;
    public GameObject barriers;

    private Button _startButton;
    private bool _ready;

    void Start()
    {
        _startButton = GetComponent<Button>();
        _startButton.onClick.AddListener(() => OnButtonHit());

        _ready = false;
    }

    void OnButtonHit()
    {
        if (!_ready)
        {
            DrawParts p1Drawer = p1DrawZone.GetComponent<DrawParts>();

            p1Drawer.isActive = true;

            p1Drawer.AttemptLoad();

            gameObject.GetComponentInChildren<Text>().text = "Start Game";

            _ready = true;
        }
        else
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        if (p1DrawZone == null)
        {
            return;
        }
        
        DrawParts drawer = p1DrawZone.GetComponent<DrawParts>();

        drawer.SavePlayer();

        List<GameObject> parts = drawer.GetParts();

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

        Camera.main.GetComponent<CamFollowSingle>().BeginGame(drawer.GetPlayer().transform);

        Destroy(p1DrawZone);
        Destroy(p1DrawText);
        Destroy(barriers);
        Destroy(gameObject);
    }
}
