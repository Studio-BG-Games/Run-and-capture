using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObj : MonoBehaviour
{
    public TileOwner owner = TileOwner.Ariost;

    public float damage = 100f;

    public GameObject collisionVFX, groundVFX;

    public float timeToDamage = 1f;

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
            playerState.SetNewState(CharacterState.Frozen);
            if (collisionVFX != null)
            {
                Instantiate(collisionVFX, collisionVFX.transform.position + transform.position, collisionVFX.transform.rotation);
            }            
            StartCoroutine(WaitToDamage(timeToDamage, healthController, playerState));

        }       
    }

    private IEnumerator WaitToDamage(float time, HealthController healthController, PlayerState player)
    {
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.fixedDeltaTime;
            player.currentState = CharacterState.Frozen;
            yield return new WaitForFixedUpdate();
        }        
        healthController.TakeDamage(damage, collisionVFX, groundVFX);
        Debug.Log("ouch");
        Destroy(gameObject);
        player.SetNewState(CharacterState.Idle);

    }
}
