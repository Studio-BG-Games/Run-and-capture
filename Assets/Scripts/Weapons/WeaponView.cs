using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DefaultNamespace;
using DefaultNamespace.Weapons;
using UnityEngine;
using Weapons;

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
        var go = Instantiate(_weapon.VFXGameObject, transform.position, transform.rotation);
        go.AddComponent<VFXView>();
        MusicController.Instance.AddAudioSource(go);
        MusicController.Instance.RemoveAudioSource(gameObject);
        MusicController.Instance.PlayAudioClip(_weapon.hitSound, go);
    }
}
