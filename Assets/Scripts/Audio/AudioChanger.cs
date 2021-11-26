using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChanger : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    public AudioClip hit_LightingAudio, hit_LaserAudio, hit_TowerFireballAudio; 
    [SerializeField] 
    private AudioClip[] hit_SFX;
    [SerializeField] 
    private AudioClip[] throw_SFX;
    private AudioClip hit_Audio, throw_Audio;
    [SerializeField] private AudioClip bonusSound;    
    [SerializeField] private AudioClip _startGame;  
    
    [SerializeField] private AudioController _controller;

    [SerializeField] private AudioSource aSourse;
    private float _startVolume;

    private void Awake() {
        //if(state.defaultAction.name == "LaserAttack")/*
        if(aSourse != null)
        {
            //aSourse = GetComponent<AudioSource>();
            _startVolume = aSourse.volume ;            
        }
        else
        {
            aSourse = GetComponent<AudioSource>();
        }


        if(bonusSound != null)
        {
             aSourse.volume = _startVolume ;
            aSourse.PlayOneShot(bonusSound);
        }

        if(_startGame != null)
        {
            aSourse.volume = _startVolume ;
            aSourse.PlayOneShot(_startGame);
        }


        hit_Audio = _controller.hit_SFX;
        throw_Audio = _controller.throw_SFX;
        

        switch (state.defaultAction.name )
        {
            case "LaserAttack":
                //_controller.hit_SFX = hit_Audio;
                _controller.throw_SFX = throw_SFX[1];
                break;
            case "StandartAttack":
                //_controller.hit_SFX = hit_Audio;
                _controller.throw_SFX = throw_SFX[0];
                break;
            case "TowerCrystallAttack":
                //_controller.hit_SFX = hit_Audio;
                _controller.throw_SFX = throw_SFX[2];
                break;
            default:
                //_controller.hit_SFX = hit_Audio;
                _controller.throw_SFX = throw_Audio;
                break;
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        aSourse.volume = _startVolume ;
        if(other.gameObject.GetComponent<ProjectileController>() != null)
        {
            
            var damage = other.gameObject.GetComponent<ProjectileController>().damage;
            switch (damage)
            {
                case 1000:
                {
                    hit_Audio = hit_LightingAudio;
                    _controller.hit_SFX = hit_Audio;
                    break;
                }
                case 360:
                {
                    hit_Audio = hit_LaserAudio;
                    _controller.hit_SFX = hit_Audio;
                    break;
                }
                case 440:
                {
                    hit_Audio = hit_TowerFireballAudio;
                    _controller.hit_SFX = hit_Audio;
                    break;
                }
                default:
                    _controller.hit_SFX = hit_Audio;
                    break;
            }            
        }


    }
}
