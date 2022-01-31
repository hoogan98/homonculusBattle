using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour
{
    public float ratio;
    
    public void Start()
    {
        ratio = GetComponent<Transform>().localScale.x / GetComponent<Renderer>().bounds.size.x;
    }
}
