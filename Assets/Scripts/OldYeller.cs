using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldYeller : MonoBehaviour
{
    public AudioClip yell;

    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.gameObject.CompareTag("Brain"))
            {
                collision.transform.parent.GetComponent<Homonculus>().PlaySound(yell);
            }

    }
}
