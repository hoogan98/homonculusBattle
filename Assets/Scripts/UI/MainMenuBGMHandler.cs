using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBGMHandler : MonoBehaviour
{
    public AudioSource bgm;
    public Slider volumeSlider;
    public Image toggleButtonImage;

    public Sprite unmutedTexture;
    public Sprite mutedTexture;

    private float originalVolume;

    // Start is called before the first frame update
    void Start()
    {
        originalVolume = bgm.volume;
    }

    public void OnSliderChange()
    {
        bgm.volume = originalVolume * (volumeSlider.value * 2);
        ToggleSprite();
    }

    public void ToggleBGM()
    {
        bgm.mute = !bgm.mute;
        ToggleSprite();
    }

    private void ToggleSprite()
    {
        if (bgm.mute || bgm.volume == 0)
        {
            toggleButtonImage.sprite = mutedTexture;
        }
        else
        {
            toggleButtonImage.sprite = unmutedTexture;
        }
    }
}
