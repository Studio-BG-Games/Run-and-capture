using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public List<AudioClip> jump_SFXs;
    public List<AudioClip> capture_SFXs;
    public AudioClip hit_SFX, throw_SFX;

    [SerializeField] private AudioSource ac;/* capSrc, hitSrc, collectSrc;*/

    private float _startVolume;

    private void Start()
    {
        _startVolume = ac.volume;
    }

    public void PlayJumpSound()
    {
        if (!GameData.isSFXAllowed)
        {
            return;
        }
        ac.volume = _startVolume;
        int sfxIndex = Random.Range(0, jump_SFXs.Count);
        ac.PlayOneShot(jump_SFXs[sfxIndex]);
    }

    public void PlayCapSound()
    {
        if (!GameData.isSFXAllowed)
        {
            return;
        }
        ac.volume = _startVolume;
        int sfxIndex = Random.Range(0, capture_SFXs.Count);
        ac.PlayOneShot(capture_SFXs[sfxIndex]);
    }

    public void PlayHitSound()
    {
        if (!GameData.isSFXAllowed)
        {
            return;
        }
        ac.volume = 0.5f*_startVolume;
        ac.PlayOneShot(hit_SFX);
    }

    public void PlayCollectSound( AudioClip clip)
    {
        if (!GameData.isSFXAllowed)
        {
            return;
        }
        ac.volume = _startVolume;
        ac.PlayOneShot(clip);
    }
    public void PlayAttackSound()
    {
        if (!GameData.isSFXAllowed)
        {
            return;
        }
        ac.volume = _startVolume;
        ac.PlayOneShot(throw_SFX);
    }


    public void PlaySound(AudioClip clip)
    {
        if (!GameData.isSFXAllowed)
        {
            return;
        }
        ac.volume = 0.4f*_startVolume;
        ac.PlayOneShot(clip);
    }
}
