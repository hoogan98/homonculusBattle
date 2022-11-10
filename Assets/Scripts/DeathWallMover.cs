using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWallMover : MonoBehaviour
{
    public Vector3 direction;

    private float speed;

    public void Start() {
        speed = PlayerPrefs.GetFloat("DeathWallSpeed", 1);
        this.enabled = false;
    }

    public void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
