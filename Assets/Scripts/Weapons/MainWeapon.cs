using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerState) )]
public class MainWeapon : MonoBehaviour
{
    private PlayerState _characterWeapon;
    private AttackEnergyController _attackEnergiController;
    private DirectWeapon _directWeapon;
 
    private float _attackResetTime;
    private float _attackCost;

    private void Awake() 
    {
        _attackEnergiController = GetComponent<AttackEnergyController>();
        
        _characterWeapon = GetComponent<PlayerState>();

        if(StateWeapon._chosenWeapon != null )
        {
            _characterWeapon.defaultAction = StateWeapon._chosenWeapon;

            _attackResetTime = StateWeapon._resetTime;
            _attackCost = StateWeapon._attackCost;
            
            _attackEnergiController.attackResetTime = _attackResetTime;
            _attackEnergiController.attackCost = _attackCost;
        }
        else
        {
            _attackResetTime = 3f;
            _attackCost = 1f;
            
            _attackEnergiController.attackResetTime = _attackResetTime;
            _attackEnergiController.attackCost = _attackCost;
        }
    }
}
