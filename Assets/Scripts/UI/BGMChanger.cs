using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMChanger : MonoBehaviour
{
    public Text assignedText;

    public void Start() {
        assignedText.text = BackgroundMusicSingleton.instance.CurrentClipName();
    }

    public void PlayNext() {
        assignedText.text = BackgroundMusicSingleton.instance.NextClip();
    }

    public void PlayPrevious() {
        assignedText.text = BackgroundMusicSingleton.instance.PreviousClip();
    }
}
