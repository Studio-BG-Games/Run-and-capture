using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaySomething : MonoBehaviour
{
    [SerializeField] private List<HealthController> playerHealths;
    void Start()
    {
        playerHealths = new List<HealthController>( FindObjectsOfType<HealthController>()); 
        //playerHealths.AddRange( );
        foreach(HealthController hc in playerHealths)
        {
            
            if(hc.currentHealth <= 0)
            {
                FindObjectOfType<AudioSwitcher>().PlayPhrase(Random.Range(0, 15).ToString());
            }
        }

    }

    // Update is called once per frame

}
