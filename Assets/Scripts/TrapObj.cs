using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObj : MonoBehaviour
{
    public TileOwner owner = TileOwner.Ariost;

    public float damage = 100f;

    public GameObject collisionVFX;

    public void SetOwner(TileOwner newOwner)
    {
        owner = newOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        var healthController = other.gameObject.GetComponent<HealthController>();
        var playerState = other.gameObject.GetComponent<PlayerState>();        
        if (healthController && owner != playerState.ownerIndex)
        {
            healthController.TakeDamage(damage);
            if(collisionVFX!=null)
            {
                Instantiate(collisionVFX, collisionVFX.transform.position + transform.position, collisionVFX.transform.rotation);
            }
            Destroy(gameObject);
            
        }       
    }
}
