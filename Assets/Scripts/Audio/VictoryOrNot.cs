using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryOrNot : MonoBehaviour
{
    [SerializeField] private GameObject greyHeart;
    //[SerializeField] private Text text;
    [SerializeField] private Extralife extralife;
    private string zero = "0";
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip defeatSound;
    [SerializeField] private AudioSource audioSource;
    public static AudioClip changeSound;
    private float volume;

    private void Awake() 
    {
        //if(extralife.life < 0)
        volume = audioSource.volume;
        changeSound = defeatSound;
        //PlayGameOver();
    }

    private void Update() {
        audioSource.volume = volume;
        if(extralife.life < 2 )
        {
            audioSource.clip = changeSound;            
        }

        else if(greyHeart.activeSelf == true)
        {
            PlayGameOver(extralife.life);
            audioSource.clip = changeSound;           
        }

        
    }

/*    private void LateUpdate() {
        //volume = 0.5f * audioSource.volume;
        //audioSource.volume = volume ;
        //PlayGameOver(extralife.life);      
    }*/
    
    private void OnTriggerEnter(Collider other) {

            //PlayGameOver(extralife.life);            
        
    }

    public void PlayGameOver( int life )
    {
        //if(extralife = null)
        //return;
        if(life <= 2)
        {
            //audioSource.volume = volume;
            
            audioSource.clip = defeatSound;
            changeSound = defeatSound;
            //audioSource.PlayOneShot(defeatSound);
            return ;
            //audioSource.PlayOneShot(audioSource.clip);

            //audioSource.clip = defeatSound;      
            //audioSource.PlayOneShot(defeatSound);      
        }
        return;
        //return defeatSound;
    }
/*
    private void OnDisable(int count) {
        count = extralife.life;
        if(count <= 0)
        {
            audioSource.volume = 0.5f * volume ;
            audioSource.PlayOneShot(defeatSound);             
        }

    }
    */
}
