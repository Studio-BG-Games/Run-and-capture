using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryOrNot : MonoBehaviour
{
    //[SerializeField] private GameObject greyHeart;
    //[SerializeField] private Extralife extralife;
    private string zero = "0";
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip defeatSound;
    [SerializeField] private AudioSource audioSource;
    private float volume;

    private void Awake() {
        volume = audioSource.volume;
    }


    public void Play()
    {
        
        
            audioSource.volume = 0.5f * volume ;
            //audioSource.clip = defeatSound;
            //audioSource.PlayOneShot(audioSource.clip);
            audioSource.PlayOneShot(defeatSound);
        
    }
}
