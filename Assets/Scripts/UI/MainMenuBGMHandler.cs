using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBGMHandler : MonoBehaviour
{
    public Slider volumeSlider;
    public Image toggleButtonImage;

    public Sprite unmutedTexture;
    public Sprite mutedTexture;

    public float baseVolume = 0.015f;
    private AudioSource bgm;

    // Start is called before the first frame update
    void Start()
    {
        bgm = BackgroundMusicSingleton.instance.AudioSourceComponent;
        bgm.mute = PlayerPrefs.GetInt("main_menu_volume_muted", 0) == 1;
        bgm.volume = baseVolume;
        bgm.volume = PlayerPrefs.GetFloat("main_menu_volume", baseVolume);
        volumeSlider.value = bgm.volume / (baseVolume * 2);
    }

    public void OnSliderChange()
    {
        bgm.volume = baseVolume * (volumeSlider.value * 2);
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
