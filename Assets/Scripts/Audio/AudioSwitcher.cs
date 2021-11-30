using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioSwitcher : MonoBehaviour
{
    public Sounds[] sounds;
    //public Sounds[] phrases;

    private void Awake() {
        foreach(Sounds s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

        Play("Startgame");
/*
        foreach(Sounds p in phrases)
        {
            p.source = gameObject.AddComponent<AudioSource>();
            p.source.clip = p.clip;

            p.source.volume = p.volume;
            p.source.pitch = p.pitch;
        }    
        */
    }


    public void Play(string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.nmae == name);
        if(s == null)
            return;
        s.source.Play();
    }
    /*
    public void PlayPhrase(string name)
    {
        Sounds s = Array.Find(phrases, sound => sound.nmae == name);
        if(s == null)
            return;
        s.source.Play();
    }
    */
}
