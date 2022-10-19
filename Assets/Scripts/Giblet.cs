using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giblet : MonoBehaviour
{
    public Sprite[] possibleSprites;
    public float velDeviation;
    public float torqueDeviation;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = possibleSprites.PickRandom<Sprite>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-velDeviation, velDeviation), Random.Range(-velDeviation, velDeviation)));
        rb.AddTorque(Random.Range(-torqueDeviation, torqueDeviation));
    }
}
