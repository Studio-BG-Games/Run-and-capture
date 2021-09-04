using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUIController : MonoBehaviour
{
    public float updateRate = 0.01f;

    [SerializeField]
    private UI_Quantity _healthUI;
    [SerializeField]
    private UI_Quantity _attackEnergyUI;

    private HealthController _healthController;
    private AttackEnergyController _attackEnergy;



    private void Awake()
    {
        _healthController = GetComponent<HealthController>();
        _attackEnergy = GetComponent<AttackEnergyController>();


        _healthController.OnHealthChanged += UpdateHealthUI;
        _attackEnergy.OnAttackEnergyChanged += UpdateEnergyUI;
    }

    private void UpdateEnergyUI(float curEnergy, float maxEnergy)
    {
        _attackEnergyUI.UpdateBar(curEnergy, maxEnergy);
    }

    private void UpdateHealthUI(float curHealth, float maxHealth)
    {
        _healthUI.UpdateBar(curHealth, maxHealth);
    }









}
