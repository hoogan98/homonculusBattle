using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWallMover : MonoBehaviour
{
    public Vector3 direction;

    private float speed;

    public void Start() {
        speed = Settings.instance.deathWallSpeed;
    }

    public void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
