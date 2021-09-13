using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public List<AudioClip> jump_SFXs;
    public List<AudioClip> capture_SFXs;
    public AudioClip hit_SFX;

    [SerializeField] private AudioSource ac;/* capSrc, hitSrc, collectSrc;*/

    private float _startVolume;

    private void Start()
    {
        _startVolume = ac.volume;
    }

    public void PlayJumpSound()
    {
        ac.volume = _startVolume;
        int sfxIndex = Random.Range(0, jump_SFXs.Count);
        ac.PlayOneShot(jump_SFXs[sfxIndex]);
    }

    public void PlayCapSound()
    {
        ac.volume = _startVolume;
        int sfxIndex = Random.Range(0, capture_SFXs.Count);
        ac.PlayOneShot(capture_SFXs[sfxIndex]);
    }

    public void PlayHitSound()
    {
        ac.volume = 0.5f*_startVolume;
        ac.PlayOneShot(hit_SFX);
    }

    public void PlayCollectSound( AudioClip clip)
    {
        ac.volume = _startVolume;
        ac.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip)
    {
        ac.volume = 0.6f*_startVolume;
        ac.PlayOneShot(clip);
    }
}
