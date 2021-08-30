using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeHealthController : MonoBehaviour
{
    public float startHealth = 2000f;
    public float currentHealth;

    public GameObject playerImpactVFX, groundImpactVFX;

    public GameObject dieVFX;

    //public Action<float, float> OnHealthChanged;

    //private TreesSpawner _treeSpawner;    

    /*private void Awake()
    {
        _treeSpawner = FindObjectOfType<TreesSpawner>();       
    }*/

    private void OnEnable()
    {
        currentHealth = startHealth;
        //OnHealthChanged?.Invoke(currentHealth, startHealth);
    }
    

    public void TakeDamage(float amount)
    {                
        //OnHealthChanged?.Invoke(currentHealth, startHealth);
        Instantiate(playerImpactVFX, transform.position, playerImpactVFX.transform.rotation);
        Instantiate(groundImpactVFX, transform.position + groundImpactVFX.transform.position, groundImpactVFX.transform.rotation);
        Damage(amount);
    }

    private void Damage(float amount)
    {
        if (currentHealth < 0)
            return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("Dead");
        }
    }

    private void Die()
    {
        Instantiate(dieVFX, transform.position + dieVFX.transform.position, dieVFX.transform.rotation);
        TreesSpawner.RemoveTree(gameObject);
    }
}
