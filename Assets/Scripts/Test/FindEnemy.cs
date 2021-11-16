using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEnemy : MonoBehaviour
{
    [SerializeField] private List<PlayerState> enemies;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.GetComponent<PlayerState>().ownerIndex != gameObject.GetComponentInParent<PlayerState>().ownerIndex)
        {
            enemies.Add(new PlayerState());
        }
    }
}
