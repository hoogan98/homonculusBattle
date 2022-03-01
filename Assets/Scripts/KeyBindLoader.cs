using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class KeyBindLoader : MonoBehaviour
{
    private List<KeyCode> _p1ControlKeys;
    private List<KeyCode> _p2ControlKeys;
    private Dictionary<DrawParts.DrawControl, KeyCode> _drawKeys;

    public void Start()
    {
        //remove these later, for the initial population of text file
        // _p1ControlKeys = new List<KeyCode>()
        // {
        //     KeyCode.UpArrow,
        //     KeyCode.DownArrow,
        //     KeyCode.LeftArrow,
        //     KeyCode.RightArrow,
        //     KeyCode.Backslash,
        //     KeyCode.RightShift
        // };
        //
        // _p2ControlKeys = new List<KeyCode>()
        // {
        //     KeyCode.W,
        //     KeyCode.S,
        //     KeyCode.A,
        //     KeyCode.D,
        //     KeyCode.Tab,
        //     KeyCode.LeftShift
        // };
        //
        // _drawKeys = new Dictionary<DrawParts.DrawControl, KeyCode>()
        // {
        //     { DrawParts.DrawControl.DeleteMode, KeyCode.LeftShift },
        //     { DrawParts.DrawControl.PlaceJoint, KeyCode.Mouse1 },
        //     { DrawParts.DrawControl.PlacePart, KeyCode.Mouse0 },
        //     { DrawParts.DrawControl.SwitchPart, KeyCode.Space }
        // };
        
        LoadKeys();
    }

    public void LoadKeys()
    {
        foreach (string control in File.ReadLines("KeyBinds.txt"))
        {
            string[] splitControls = control.Split(',');

            switch (splitControls[0])
            {
                case "playerOne":
                    _p1ControlKeys = InterpretKeys(splitControls);
                    break;
                    
                case "playerTwo":
                    _p2ControlKeys = InterpretKeys(splitControls);
                    break;
                
                case "drawControl":
                    string[] keys = new string[splitControls.Length - 1];
                    string[] controls = new string[splitControls.Length - 1];
                    
                    for (int i = 1; i < splitControls.Length; i++)
                    {
                        string[] splitPair = splitControls[i].Split(':');

                        controls[i - 1] = splitPair[0];
                        keys[i - 1] = splitPair[1];
                    }

                    _drawKeys = InterpretKeys(keys, controls);
                    break;
            }
        }
    }

    private List<KeyCode> InterpretKeys(string[] keys)
    {
        List<KeyCode> keyList = new List<KeyCode>();

        for (int i = 1; i < keys.Length; i++)
        {
            keyList.Add((KeyCode)Enum.Parse(typeof(KeyCode), keys[i]));
        }

        return keyList;
    }

    private Dictionary<DrawParts.DrawControl, KeyCode> InterpretKeys(string[] keys, string[] controls)
    {
        Dictionary<DrawParts.DrawControl, KeyCode> keyDict = new Dictionary<DrawParts.DrawControl, KeyCode>();

        for (int i = 0; i < keys.Length; i++)
        {
            keyDict.Add((DrawParts.DrawControl)Enum.Parse(typeof(DrawParts.DrawControl), controls[i]),
                (KeyCode)Enum.Parse(typeof(KeyCode), keys[i]));
        }

        return keyDict;
    }

    public void WriteKeys()
    {
        string[] keyStrings = new string[3];

        keyStrings[0] = "playerOne" + StringifyKeys(_p1ControlKeys);
        keyStrings[1] = "playerTwo" + StringifyKeys(_p2ControlKeys);
        keyStrings[2] = "drawControl" + StringifyKeys(_drawKeys);


        File.WriteAllLines("KeyBinds.txt", keyStrings);
    }

    private string StringifyKeys(List<KeyCode> keys)
    {
        string ret = "";

        foreach (KeyCode key in keys)
        {
            ret += "," + key;
        }

        return ret;
    }

    private string StringifyKeys(Dictionary<DrawParts.DrawControl, KeyCode> keys)
    {
        string ret = "";

        foreach (DrawParts.DrawControl control in keys.Keys)
        {
            ret += "," + control + ":" + keys[control];
        }

        return ret;
    }
}