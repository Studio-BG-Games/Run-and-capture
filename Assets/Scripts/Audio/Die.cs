using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField] private HealthController heakth;
    [SerializeField] private AudioClip deadClip;
    [SerializeField] private AudioSource source;
    private float volume;
    // Start is called before the first frame update
    void Start()
    {
        volume = source.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if(heakth.enabled == false)
        {
            source.volume = volume;
            source.Play();
        }
    }
}
