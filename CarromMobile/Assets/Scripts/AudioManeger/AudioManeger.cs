using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManeger : MonoBehaviour
{
    public Sound[] sounds;
    public float musicVolume;
    public float sfxVolume;
    public static AudioManeger audioManegerInstance;
    // Start is called before the first frame update
    void Awake()
    {
        if (audioManegerInstance == null)
            audioManegerInstance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        musicVolume = 1f;
        sfxVolume = 1f;
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spartielBlend;


        }
    }


    public void Play(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.volume = volume * sfxVolume;
        s.source.Play();
    }

   



}
