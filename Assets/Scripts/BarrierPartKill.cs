using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPartKill : MonoBehaviour
{
    string[] partTags = new string[]{
        "Bone",
        "Muscle",
        "Brain"
    };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in partTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                collision.gameObject.GetComponent<Part>().Die();
                return;
            }
        }

    }
}
