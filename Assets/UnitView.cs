using System;
using DefaultNamespace.Weapons;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    public Action OnStep;
    public Action OnAttackEnd;
    public Action OnAttack;
    public Action<float> OnHit;
    public GameObject charBarCanvas;

    private void Step()
    {
        OnStep?.Invoke();
    }

    private void AttackEnd()
    {
        OnAttackEnd?.Invoke();
    }

    private void Attack()
    {
        OnAttack?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponView weaponView = other.GetComponent<WeaponView>();
        if (weaponView != null)
        {
            OnHit?.Invoke(weaponView.Weapon.damage);
            Destroy(other);
        }
    }

    
}