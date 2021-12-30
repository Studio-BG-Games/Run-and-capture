using System.Collections.Generic;
using Data;
using UnityEngine;

public class MusicController
{
    public static MusicController Instance { get; private set; }
    public MusicData MusicData => _data;

    private MusicData _data;
    private AudioListener _audioListener;
    private Dictionary<GameObject, AudioSource> _sources;

    public MusicController()
    {
        Instance ??= this;
        _sources = new Dictionary<GameObject, AudioSource>();
    }
        
    public void SetMusicData(MusicData data)
    {
        _data = data;
    }

    public void PlayAudioClip(AudioClip clip, GameObject source)
    {
        _sources[source].clip = clip;
        _sources[source].volume = _data.Settings.isSFXAllowed ? 1f : 0f;
        _sources[source].Play();
    }

    public void PlayRandomClip(List<AudioClip> clips, GameObject source)
    {
        _sources[source].clip = clips[Random.Range(0, clips.Count - 1)];
        _sources[source].volume = _data.Settings.isSFXAllowed ? 1f : 0f;
        _sources[source].Play();
    }

    public void AddAudioListener(GameObject gameObject)
    {
        _audioListener = gameObject.AddComponent<AudioListener>();
    }

    public void AddAudioSource(GameObject gameObject)
    {
        var source = gameObject.AddComponent<AudioSource>();
        _sources.Add(gameObject, source);
    }

    public void RemoveAudioSource(GameObject gameObject)
    {
        _sources.Remove(gameObject);
    }
}