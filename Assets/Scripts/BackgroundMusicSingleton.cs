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

    private List<AudioClip> allAudio;

    private int currentClip;

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

        allAudio = new List<AudioClip>();
        allAudio.AddRange(Resources.LoadAll<AudioClip>("music"));

        SceneMusic sceneMusic = SceneMusicMap.First(value => value.sceneName == SceneManager.GetActiveScene().name);

        currentClip = allAudio.IndexOf(sceneMusic.audioClip);
    }

    public void NextClip() {
        currentClip++;

        if (currentClip >= allAudio.Count) {
            currentClip = 0;
        }

        for (int i = 0; i < SceneMusicMap.Length; i++)
        {
            SceneMusicMap[i].audioClip = allAudio[currentClip];
        }
    }

    public void PreviousClip() {
        currentClip--;

        if (currentClip < 0) {
            currentClip = allAudio.Count - 1;
        }

        for (int i = 0; i < SceneMusicMap.Length; i++)
        {
            SceneMusicMap[i].audioClip = allAudio[currentClip];
        }
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
