using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPartKill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bone") || collision.gameObject.CompareTag("Muscle"))
        {
            Destroy(collision.gameObject);
        }
    }
}
