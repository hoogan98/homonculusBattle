using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonHandler : MonoBehaviour
{
    public GameObject p1DrawZone;
    public GameObject p2DrawZone;
    public GameObject barriers;
    public Text p1DrawText;
    public Text p2DrawText;
    public float gravity;

    private Button _startButton;

    void Start()
    {
        _startButton = GetComponent<Button>();
        _startButton.onClick.AddListener(() => OnButtonHit());
    }

    public void OnButtonHit()
    {
        if (p1DrawZone == null || p2DrawZone == null)
        {
            return;
        }
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
                part.GetComponent<BoxCollider2D>().isTrigger = false;
            }
            else
            {
                part.GetComponent<Muscle>().StartGame();
            }
            
            Destroy(part.GetComponent<DrawingBehavior>());
        }

        Destroy(p1DrawZone);
        Destroy(p2DrawZone);
        Destroy(p1DrawText);
        Destroy(p2DrawText);
        Destroy(barriers);
        Destroy(gameObject);
    }
}
