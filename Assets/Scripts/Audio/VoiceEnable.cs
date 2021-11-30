using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceEnable : MonoBehaviour
{
    public GameObject objectToEnable;
    public GameObject objectAiToEnable;
    public AudioSource source;
    public AudioSource aiSource;
    public static bool isDisable;
    public static bool isAiDisabled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDisable)
            objectToEnable.SetActive(true);
            
        else
            objectToEnable.SetActive(false);
            //source.PlayOneShot(source.clip);
            //source.playOnAwake = source.clip;

        //if(objectAiToEnable != null)
        //{
            if(isAiDisabled)
                objectAiToEnable.SetActive(true);
                
            else
                objectAiToEnable.SetActive(false);            
        //}

    }
}
