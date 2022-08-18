using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrevBuildRemover : MonoBehaviour
{
    public GameObject blankHomonculus;

    public void OnClick()
    {
        GameObject[] prevBuilds = Resources.LoadAll<GameObject>("Builds/");

        foreach (GameObject build in prevBuilds) {
            PrefabUtility.SaveAsPrefabAsset(blankHomonculus, "Assets/Resources/Builds/" + build.name + ".prefab");
        }   
    }
}
