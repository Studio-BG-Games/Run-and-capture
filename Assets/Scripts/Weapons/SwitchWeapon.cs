using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum TypeOfWeapon
    {
        Bolt,
        Laser,
        Random
    }
public class SwitchWeapon : MonoBehaviour
{    
    // public enum TypeOfWeapon
    // {
    //     Bolt,
    //     Laser,
    //     Random
    // }

    [SerializeField] private TypeOfWeapon _weaponType;
    [SerializeField] private List<PlayerAction> _weaponlist = new List<PlayerAction>();
    private PlayerState _playerState;
    private AttackEnergyController _attackEnergiController; // = new AttackEnergyController();

    //public float ResetTime { get { return _attackEnergiController.attackResetTime;} set {value = _attackEnergiController.attackResetTime; }}

    private void Awake() 
    {
        _attackEnergiController = GetComponent<AttackEnergyController>();
        
        _playerState = GetComponent<PlayerState>();
        //_weapon = StateWeapon._chosenWeapon;
        ChangehWeapon(_weaponType);
    }

    private void ChangehWeapon(TypeOfWeapon weapon)
    {
            if(weapon == TypeOfWeapon.Bolt)
            {
                MakeWeapon(0);
            }

            if (weapon == TypeOfWeapon.Laser)
            {
                MakeWeapon(1);
            }

            if(weapon == TypeOfWeapon.Random)
            {
                MakeWeapon(Random.Range(0, _weaponlist.Count));
            }
    }

    private void MakeWeapon(int weaponIndex)
    {
        _playerState.defaultAction = _weaponlist[weaponIndex];
        
            if(weaponIndex == 0)
            {      
                _attackEnergiController.attackResetTime = 3f; 
                _attackEnergiController.attackCost = 1f; 
            }

            if(weaponIndex == 1)
            {
                _attackEnergiController.attackResetTime = 2f;
                _attackEnergiController.attackCost = 0.5f;
            }
    }

    public void Switcher(PlayerAction weaponScript)
    {
        
    }
}


