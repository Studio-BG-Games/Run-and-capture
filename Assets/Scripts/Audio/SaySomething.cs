using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
using UnityEngine.Audio;


public class SaySomething : MonoBehaviour
{
    //[SerializeField] private List<HealthController> playerHealths;
    [SerializeField] private List<Sounds> prases;
    [SerializeField] private HealthController health;

    void Start()
    {
        health = FindObjectOfType<HealthController>();

        foreach(Sounds p in prases)
        {
            p.source = gameObject.AddComponent<AudioSource>();
            p.source.clip = p.clip;

            p.source.volume = p.volume;
            p.source.pitch = p.pitch;
        }

        // playerHealths = new List<HealthController>( FindObjectsOfType<HealthController>()); 
        // //playerHealths.AddRange( );
        // foreach(HealthController hc in playerHealths)
        // {
            
        //     if(hc.currentHealth <= 0)
        //     {
        //         //FindObjectOfType<AudioSwitcher>().PlayPhrase(Random.Range(0, 15));
        //     }
        // }

    }

    // Update is called once per frame

    private void Update() {
        if(health.currentHealth <= 0)
        {
            Say();
        }
    }

    public void Say()
    {
        
        if()
    }
}
