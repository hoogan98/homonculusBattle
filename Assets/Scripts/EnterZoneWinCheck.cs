using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterZoneWinCheck : MonoBehaviour
{
    public AudioClip winSound;
    public Button menuButton;
    public Button againButton;
    public GameObject canvas;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Brain"))
        {
            other.gameObject.GetComponentInParent<Homonculus>().PlaySound(winSound);
            Instantiate(againButton, canvas.transform);
            Instantiate(menuButton, canvas.transform);
            Destroy(gameObject.GetComponent<Collider2D>());

            try
            {
                GetComponent<LevelUpHandler>().LevelUp();
            }
            catch
            {
                Debug.Log("no level up handler found");
            }
        }
    }
}
