using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float startHealth = 2000f;
    public float currentHealth;    

    public GameObject playerImpactVFX, groundImpactVFX;

    public Action<float, float> OnHealthChanged;

    private DeathChecker _deathChecker;
    private PlayerState _playerState;
    
    private void Awake()
    {
        _deathChecker = FindObjectOfType<DeathChecker>();
        _playerState = GetComponent<PlayerState>();
    }

    private void OnEnable()
    {
        currentHealth = startHealth;
        OnHealthChanged?.Invoke(currentHealth, startHealth);
    }

    public void TakeDamage(float amount, GameObject plImpactVFX, GameObject grndImpactVFX)
    {
        if (currentHealth < 0)
            return;
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth, startHealth);
        
        playerImpactVFX = plImpactVFX;
        groundImpactVFX = grndImpactVFX;
        
        Instantiate(playerImpactVFX, transform.position, playerImpactVFX.transform.rotation);
        Instantiate(groundImpactVFX, transform.position+Vector3.up*0.01f, groundImpactVFX.transform.rotation);
        
        if (currentHealth <= 0)
        {
            Die();
            //Debug.Log("Dead");
        }
    }

    private void Die()
    {
        //_deathChecker.MakeDead(_playerState);
        _deathChecker.MakeDeadPermanent(_playerState);
    }
}
