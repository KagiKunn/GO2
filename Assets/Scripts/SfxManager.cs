using System;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

#pragma warning disable CS0108, CS0114

public class SfxManager : MonoBehaviour {
    public AudioClip button;
    public AudioSource audio;
    
    public static SfxManager Instance;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }

    public void clickSound()
    {
        audio.Play();
        CustomLogger.LogWarning("Clicked Sound");
    }
}