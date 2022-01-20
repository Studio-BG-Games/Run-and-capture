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
        _sources[source].volume = _data.Settings.sfxVolume;
        _sources[source].spatialBlend = 1f;
        _sources[source].Play();
    }

    public void PlayRandomClip(List<AudioClip> clips, GameObject source)
    {
        if (!_sources.TryGetValue(source, out var value)) return;
        
        value.clip = clips[Random.Range(0, clips.Count - 1)];
        value.volume = _data.Settings.sfxVolume;
        value.spatialBlend = 1f;
        value.Play();
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
        if (_sources.ContainsKey(gameObject))
            _sources.Remove(gameObject);
    }
}