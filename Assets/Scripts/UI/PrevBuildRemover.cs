using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PrevBuildRemover : MonoBehaviour
{
    public void OnClick()
    {
        System.IO.DirectoryInfo di = new DirectoryInfo("Assets/Resources/Builds/");

        foreach (FileInfo file in di.GetFiles())
        {
            //file.Delete();
            Debug.Log(file.Name);
        }
    }
}
