using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Gameover : MonoBehaviour
{
    //public AudioSource source;
    public GameObject fake;
    public static bool disable = true;
    

    // Update is called once per frame
    void Update()
    {
        
        if(disable)
        {
            fake.SetActive(false);
            
        }
        else
        {
            fake.SetActive(true);
        }
    }
}
