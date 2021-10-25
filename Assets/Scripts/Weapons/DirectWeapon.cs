using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectWeapon : MonoBehaviour
{
    [SerializeField] private List<PlayerAction> _weaponlist = new List<PlayerAction>();
    private AttackEnergyController _attackEnergiController;

    public void SetWeapon(int index)
    {
        //_index = index;
        StateWeapon._chosenWeapon = _weaponlist[index];
            
            if(index == 0)
            {      
                StateWeapon._resetTime = 3f; 
                StateWeapon._attackCost = 1f; 
            }

            if(index == 1)
            {
                StateWeapon._resetTime = 2f;
                StateWeapon._attackCost = 0.5f;
            }
    }
    
}
