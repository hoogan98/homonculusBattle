using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicSingleton : MonoBehaviour
{
    [Serializable]
    public struct SceneMusic
    {
        public string sceneName;
        public AudioClip audioClip;
    }
    
    public static BackgroundMusicSingleton instance;

    public SceneMusic[] SceneMusicMap;

    public AudioSource AudioSourceComponent => GetComponent<AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += ReplaceMusic;
    }

    private void ReplaceMusic(Scene activeScene, LoadSceneMode mode)
    {
        var sceneName = activeScene.name;
        var sceneMusic = SceneMusicMap.First(value => value.sceneName == sceneName);

        if (AudioSourceComponent.clip.name != sceneMusic.audioClip.name)
        {
            AudioSourceComponent.clip = sceneMusic.audioClip;
            AudioSourceComponent.Stop();
            AudioSourceComponent.Play();
        }
        
        if (SceneMusicMap.Any(value => value.sceneName == SceneManager.GetActiveScene().name)) return;
        instance = null;
        Destroy(gameObject);
    }
}
