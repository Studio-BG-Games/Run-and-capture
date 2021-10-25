using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainWeapon : MonoBehaviour
{
    [SerializeField] private PlayerAction _weapon;
    private PlayerState _characterWeapon;
    private AttackEnergyController _attackEnergiController;
    private DirectWeapon _directWeapon;
    //private StateWeapon _stateWeapon;

    private float _attackResetTime;
    private float _attackCost;

    private void Awake() 
    {
        _attackEnergiController = GetComponent<AttackEnergyController>();
        
        _weapon = StateWeapon._chosenWeapon;
        _characterWeapon = GetComponent<PlayerState>();
        

        _characterWeapon.defaultAction = _weapon;


        _attackResetTime = StateWeapon._resetTime;
        _attackCost = StateWeapon._attackCost;
        
        _attackEnergiController.attackResetTime = _attackResetTime;
        _attackEnergiController.attackCost = _attackCost;

        // _attackEnergiController.attackResetTime = 2f;
        // _attackEnergiController.attackCost = 0.5f;
    }

    
}
