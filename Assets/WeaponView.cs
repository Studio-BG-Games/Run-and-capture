using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Weapons;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    private Weapon _weapon;

    public Weapon Weapon => _weapon;
    
    public void SetWeapon(Weapon weapon)
    {
        _weapon = weapon;
    }

    private void OnDestroy()
    {
        Instantiate(_weapon.VFXGameObject, transform.position, transform.rotation);
    }
}
