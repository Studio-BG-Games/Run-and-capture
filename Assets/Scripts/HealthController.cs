using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float startHealth = 2000f;
    public float currentHealth;

    [SerializeField]
    private UI_Quantity healthUI;

    public GameObject playerImpactVFX, groundImpactVFX;


    private void OnEnable()
    {
        currentHealth = startHealth;
        healthUI.UpdateBar(currentHealth, startHealth);
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth < 0)
            return;
        currentHealth -= amount;
        healthUI.UpdateBar(currentHealth, startHealth);
        Instantiate(playerImpactVFX, transform.position, playerImpactVFX.transform.rotation);
        Instantiate(groundImpactVFX, transform.position, groundImpactVFX.transform.rotation);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PlayerState playerState = GetComponent<PlayerState>();
        PlayerDeathController deathController = FindObjectOfType<PlayerDeathController>();
        deathController.MakeDead(playerState);
    }
}
