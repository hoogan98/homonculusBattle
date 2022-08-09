using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PrevBuildRemover : MonoBehaviour
{
    public void OnClick()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Builds/");

        foreach (FileInfo file in dir.GetFiles())
        {
            Debug.Log("deleted file: " + file.Name);
            file.Delete();
        }
    }
}
