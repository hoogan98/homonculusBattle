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
        _p1ControlKeys = new List<KeyCode>()
        {
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.Backslash,
            KeyCode.RightShift
        };

        _p2ControlKeys = new List<KeyCode>()
        {
            KeyCode.W,
            KeyCode.S,
            KeyCode.A,
            KeyCode.D,
            KeyCode.Tab,
            KeyCode.LeftShift
        };

        _drawKeys = new Dictionary<DrawParts.DrawControl, KeyCode>()
        {
            { DrawParts.DrawControl.DeleteMode, KeyCode.LeftShift },
            { DrawParts.DrawControl.PlaceJoint, KeyCode.Mouse1 },
            { DrawParts.DrawControl.PlacePart, KeyCode.Mouse0 },
            { DrawParts.DrawControl.SwitchPart, KeyCode.Space }
        };
        
        WriteKeys();
    }

    public void WriteKeys()
    {
        string[] keys = new string[3];

        keys[0] = "playerOne" + StringifyKeys(_p1ControlKeys);
        keys[1] = "playerTwo" + StringifyKeys(_p2ControlKeys);
        keys[3] = "drawControl" + StringifyKeys(_drawKeys);


        File.WriteAllLines("KeyBinds.txt", keys);
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