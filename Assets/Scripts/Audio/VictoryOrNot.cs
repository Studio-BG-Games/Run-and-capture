using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryOrNot : MonoBehaviour
{
    //[SerializeField] private GameObject greyHeart;
    [SerializeField] private Extralife extralife;
    //private string zero = "0";
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip defeatSound;
    [SerializeField] private AudioSource audioSource;
    private float volume;

    private void Awake() 
    {
        //if(extralife.life < 0)
        volume = 0.5f * audioSource.volume;
        //PlayGameOver();
    }

    private void Update() {
        if(extralife.life < -1)
        {
            PlayGameOver();            
        }
        //volume = 0.5f * audioSource.volume;

    }





    public void PlayGameOver()
    {
        
        
            audioSource.volume = 0.5f * volume ;
            //audioSource.clip = defeatSound;
            //audioSource.PlayOneShot(audioSource.clip);
            audioSource.PlayOneShot(defeatSound);
        
    }
}
