using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Weapons;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    public Action OnStep;
    public Action OnAttackEnd;
    public Action OnAttack;
    public Action<int> OnHit;
    [SerializeField] private GameObject barCanvas;
    [SerializeField] private GameObject aimCanvas;

    private Stack<ShotUIView> _shotUIStack;
    private Stack<ShotUIView> _toReloadStack;
    private Weapon _weapon;
    private int _manaRegen;
    private Action _startRegen;
    private Coroutine _previosRegen;
    private Coroutine _previosReload;
    private int _mana;

    public GameObject BarCanvas => barCanvas;
    public GameObject AimCanvas => aimCanvas;

    public void SetUp(Stack<ShotUIView> shots, Weapon weapon, Action RegenMana, int manaRegen)
    {
        _shotUIStack = shots;
        _weapon = weapon;
        _toReloadStack = new Stack<ShotUIView>();
        _startRegen = RegenMana;
        _manaRegen = manaRegen;
    }

    public bool Shoot()
    {
        if (_shotUIStack.Count == 0) return false;
        var shot = _shotUIStack.Pop();
        shot.Switch();
        _toReloadStack.Push(shot);
        if (_previosReload != null)
        {
            StopCoroutine(_previosReload);
        }
        _previosReload = StartCoroutine(Reload());
        return true;
    }

    public void RegenMana(int mana)
    {
        if (_previosRegen != null)
        {
            StopCoroutine(_previosRegen);
        }

        _mana = mana;
        _previosRegen = StartCoroutine(Regen());
    }

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
            other.transform.DOComplete();
            
            other.transform.position = transform.position;
            
            Destroy(other.gameObject);
        }
    }

    private IEnumerator Reload()
    {
        if (_toReloadStack.Count == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(_weapon.reloadTime);
        var shot = _toReloadStack.Pop();
        shot.Switch();
        _shotUIStack.Push(shot);
        StartCoroutine(Reload());
    }

    private IEnumerator Regen()
    {
        if (_mana >= 100)
        {
            yield break;
        }
        yield return new WaitForSeconds(1f);
        _mana += _manaRegen;
        _startRegen.Invoke();
        StartCoroutine(Regen());
    }
}