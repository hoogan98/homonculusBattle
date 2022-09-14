using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousHomonculus : MonoBehaviour
{
    public static PreviousHomonculus instance;

    public GameObject go;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
