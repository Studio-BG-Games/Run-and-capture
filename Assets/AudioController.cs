using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip jump_SFX, capture_SFX, hit_SFX;

    [SerializeField] private AudioSource ac;/* capSrc, hitSrc, collectSrc;*/

    public void PlayJumpSound()
    {
        ac.volume = 1f;
        ac.PlayOneShot(jump_SFX);
    }

    public void PlayCapSound()
    {
        ac.volume = 1f;
        ac.PlayOneShot(capture_SFX);
    }

    public void PlayHitSound()
    {
        ac.volume = 0.5f;
        ac.PlayOneShot(hit_SFX);
    }

    public void PlayCollectSound( AudioClip clip)
    {
        ac.volume = 1f;
        ac.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip)
    {
        ac.volume = 0.6f;
        ac.PlayOneShot(clip);
    }
}
