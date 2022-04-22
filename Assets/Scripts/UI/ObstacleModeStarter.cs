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

        p1DrawZone.GetComponent<DrawParts>().SavePlayer();

        List<GameObject> parts = p1DrawZone.GetComponent<DrawParts>().GetParts();

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
            // if (!part.CompareTag("Muscle"))
            // {
            //     part.GetComponent<Rigidbody2D>().gravityScale = gravity;
            // }
            // else
            // {
            //     part.GetComponent<Muscle>().StartGame();
            // }

            // Destroy(part.GetComponent<DrawingBehavior>());
            // part.GetComponent<BoxCollider2D>().isTrigger = false;
            // part.GetComponent<Part>().StartGame();
        }

        Destroy(p1DrawZone);
        Destroy(p1DrawText);
        Destroy(barriers);
        Destroy(gameObject);
    }
}
