using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Gameover : MonoBehaviour
{
    public AudioSource source;
    public GameObject fake;

    // Update is called once per frame
    void Update()
    {
        if(fake.activeSelf == true  && Extralife.staticLives < 1  )
        {
            source.Play();
        }
    }
}
