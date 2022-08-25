using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PrevBuildRemover : MonoBehaviour
{
    public void OnClick()
    {
        try {
            Destroy(PreviousHomonculus.instance.go);
        } catch {
            Debug.Log("no old builds to kill");
        }
    }
}
