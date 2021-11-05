using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    //[SerializeField] private Collider _aria;
    [SerializeField] private PlayerState _enemies;
    private void OnTriggerEnter(Collider other) 
    {
        if(GameObject.FindObjectOfType<DirectOwner>())
        {
            _enemies.enemies.Add(other.gameObject.GetComponent<PlayerState>());
        }
    }
}
