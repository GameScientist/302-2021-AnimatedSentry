using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// Plays sounds when asked to by other objects.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake() // Each sound effect is stored and allowed to have their volume and pitch altered.
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    // Update is called once per frame
    public void Play (string name) // Plays the selected sound.
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        s.source.Play();
    }

    public void Stop(string name) // Stops the selected sound.
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        s.source.Stop();
    }
}
