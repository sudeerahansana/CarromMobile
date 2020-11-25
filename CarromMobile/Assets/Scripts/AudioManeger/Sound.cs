using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public float volume;
    public float pitch;
    public bool loop;
    public float spartielBlend;


    [HideInInspector]
    public AudioSource source;

}
