using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainMenuBGMHandler))]
public class BGMInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Clear Preferences"))
        {
            PlayerPrefs.DeleteKey("main_menu_volume_muted");
            PlayerPrefs.DeleteKey("main_menu_volume");
        }
    }
}
