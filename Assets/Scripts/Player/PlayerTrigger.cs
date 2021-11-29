using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    //[SerializeField] private HealthController _healthHit;
    //[SerializeField] private AudioSwitcher 

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<ProjectileController>().damage == 1000)
        {
            FindObjectOfType<AudioSwitcher>().Play("Lighting_Hit");
            Debug.Log("Take 1000");
        }
        else if(other.gameObject.GetComponent<ProjectileController>().damage == 360)
        {
            FindObjectOfType<AudioSwitcher>().Play("Laser_Hit");
            Debug.Log("Take 360");
        }
        else if(other.gameObject.GetComponent<ProjectileController>().damage == 440)
        {
            FindObjectOfType<AudioSwitcher>().Play("TowerCrystall_Hit");
            Debug.Log("Take 440");
        }
    }
}
