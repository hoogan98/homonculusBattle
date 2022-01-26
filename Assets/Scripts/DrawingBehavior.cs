using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Bone") || collision.gameObject.tag.Equals("Muscle"))
        {
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }
}
