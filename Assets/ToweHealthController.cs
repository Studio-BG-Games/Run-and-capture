using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToweHealthController : MonoBehaviour
{
    public TileOwner owner;

    public float startHealth = 2000f;
    public float currentHealth;

    public GameObject playerImpactVFX, groundImpactVFX;    

    private void OnEnable()
    {
        currentHealth = startHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth < 0)
            return;
        currentHealth -= amount;
        Instantiate(playerImpactVFX, transform.position, playerImpactVFX.transform.rotation);
        Instantiate(groundImpactVFX, transform.position + Vector3.up * 0.01f, groundImpactVFX.transform.rotation);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
