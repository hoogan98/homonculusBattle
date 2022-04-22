using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControlChangerButton : MonoBehaviour
{
    public int playerID;
    public int index;
    public KeyBindLoader keyBindLoader;
    private bool _polling;

    private Text _buttonText;

    private readonly KeyCode[] MODIFIER_KEYS = { KeyCode.LeftAlt, KeyCode.RightAlt };
    // Start is called before the first frame update
    void Start()
    {
        _buttonText = GetComponentInChildren<Button>().GetComponentInChildren<Text>();
        _buttonText.text = keyBindLoader.GetKey(playerID, index).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_polling) return;
        KeyCode pressedKey = GetButtonPressed();
        if (pressedKey == KeyCode.Escape)
        {
            _polling = false;
            _buttonText.text = keyBindLoader.GetKey(playerID, index).ToString();
        }
        else if (pressedKey != KeyCode.None)
        {
            keyBindLoader.SetKey(playerID, index, pressedKey);
            _polling = false;
            _buttonText.text = keyBindLoader.GetKey(playerID, index).ToString();
        }
    }
    
    KeyCode GetButtonPressed()
    {
        KeyCode pressedKey = KeyCode.None;
        foreach(KeyCode vKey in Enum.GetValues(typeof(KeyCode))){
            if(Input.GetKey(vKey))
            {
                pressedKey = vKey;
            }
        }

        return pressedKey;
    }

    public void StartPolling()
    {
        if (_polling)
        {
            return;
        }

        //_buttonText.text = "...";
        _buttonText.text = "<Press ESC to cancel>";
        _polling = true;
    }
}
