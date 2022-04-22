using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterZoneWinCheck : MonoBehaviour
{
    public Text winText;
    public Button menuButton;
    public Button againButton;
    public GameObject canvas;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Brain")) {
            winText.text = "You Win";
             Instantiate(againButton, canvas.transform);
            Instantiate(menuButton, canvas.transform);
        }
    }
}
