using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Music {
    public AudioClip clip;

    public string name;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;

    public string tag;
}
