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
        bgm.mute = PlayerPrefs.GetInt("main_menu_volume_muted", 0) == 1;
        originalVolume = bgm.volume;
        bgm.volume = PlayerPrefs.GetFloat("main_menu_volume", originalVolume);
        volumeSlider.value = bgm.volume / (originalVolume * 2);
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
        PlayerPrefs.SetFloat("main_menu_volume", bgm.volume);
        PlayerPrefs.SetInt("main_menu_volume_muted", bgm.mute ? 1 : 0);
    }
}
