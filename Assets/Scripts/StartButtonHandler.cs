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

    private Button startButton;

    void Start()
    {
        startButton = this.GetComponent<Button>();
        startButton.onClick.AddListener(() => OnButtonHit());
    }

    public void OnButtonHit()
    {
        if (this.p1DrawZone == null || this.p2DrawZone == null)
        {
            return;
        }
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
                part.GetComponent<BoxCollider2D>().isTrigger = false;
            }
            else
            {
                part.GetComponent<MuscleBehavior>().StartGame();
            }
            
            Destroy(part.GetComponent<DrawingBehavior>());
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
                part.GetComponent<BoxCollider2D>().isTrigger = false;
            }
            else
            {
                part.GetComponent<MuscleBehavior>().StartGame();
            }
            Destroy(part.GetComponent<DrawingBehavior>());
        }

        Destroy(p1DrawZone);
        Destroy(p2DrawZone);
        Destroy(p1DrawText);
        Destroy(p2DrawText);
        Destroy(barriers);
        Destroy(this.gameObject);
    }
}
