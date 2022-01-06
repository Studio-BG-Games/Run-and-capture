using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HexFiled;
using Items;
using Units;
using UnityEngine;
using UnityEngine.UI;
using Weapons;


public class UnitView : MonoBehaviour
{
    public Action OnStep;
    public Action OnAttackEnd;
    public Action OnAttack;
    public Action<int> OnHit;
    [SerializeField] private GameObject barCanvas;
    [SerializeField] private GameObject aimCanvas;
    [SerializeField] private Image captureBar;


    private Stack<ShotUIView> _shootUIStack;
    private Stack<ShotUIView> _toReloadStack;
    private Weapon _weapon;
    private int _manaRegen;
    private Action _startRegen;
    private Coroutine _previosRegen;
    private Coroutine _previosReload;
  
    private int _mana;
    private Action _capureHex;
    private Sequence _sequence;
    private AudioSource _audioSource;
    private Unit _unit;

    public GameObject BarCanvas => barCanvas;
    public GameObject AimCanvas => aimCanvas;
    public UnitColor Color => _unit.Color;

    public void SetUp(Stack<ShotUIView> shots, Weapon weapon, Action regenMana, int manaRegen, Action captureHex,
        Unit unit)
    {
        _shootUIStack = shots;
        _weapon = weapon;
        _toReloadStack = new Stack<ShotUIView>();
        _startRegen = regenMana;
        _manaRegen = manaRegen;
        _capureHex = captureHex;
        _unit = unit;
    }

    public void HardCaptureHex(HexCell cell)
    {
        captureBar.gameObject.SetActive(true);
        _sequence = DOTween.Sequence();
        _sequence.Append(captureBar.DOFillAmount(1f, 0f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _capureHex?.Invoke();
            captureBar.DOFillAmount(0f, 0f).SetEase(Ease.Linear);
            captureBar.gameObject.SetActive(false);
            MusicController.Instance.PlayRandomClip(MusicController.Instance.MusicData.SfxMusic.Captures,
                cell.gameObject);
        }));
    }


    public void StopHardCapture()
    {
        _sequence.Kill();
        captureBar.DOFillAmount(0f, 0f).SetEase(Ease.Linear);
        captureBar.gameObject.SetActive(false);
    }

    public bool Shoot()
    {
        if (_shootUIStack.Count == 0) return false;
        var shot = _shootUIStack.Pop();
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
        //_startRegen.Invoke();  
        _previosRegen = StartCoroutine(Regen());

        //return _mana;
    }

    private void Step()
    {
        OnStep?.Invoke();
    }

    private void Land()
    {
        MusicController.Instance.PlayRandomClip(
            MusicController.Instance.MusicData.SfxMusic.Step, gameObject);
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
            OnHit?.Invoke(weaponView.Weapon.modifiedDamage);
            other.transform.DOKill();
            Destroy(other.gameObject);
        }

        ItemView itemView = other.GetComponent<ItemView>();

        if (itemView != null && _unit.PickUpItem(itemView))
        {
            Destroy(itemView.gameObject);
        }
    }

    private IEnumerator Reload()
    {
        if (_toReloadStack.Count == 0) yield break; //TODO При частой стрльбе перезарядка работает некорректно 
        yield return new WaitForSeconds(_weapon.reloadTime);
        if (_toReloadStack.Count == 0) yield break;
        var shot = _toReloadStack.Pop();
        
        // _shootUIStack.Push(shot);
            shot.Switch();
            _shootUIStack.Push(shot);       
                      
        foreach (var item in _toReloadStack)
        {
            if(Time.deltaTime < _weapon.reloadTime)
            {
                StopCoroutine(_previosReload);
                _previosReload = null;
            }
            _previosReload = StartCoroutine(Reload());    
            
        }

        

    }

    private IEnumerator Regen()
    {
        if (_mana >= 100) 
        {
            yield break;
        }


        yield return new WaitForSeconds(2f);
        while(_mana < 100)
        {
            _mana += _manaRegen;
            _startRegen.Invoke();            
        }

        //StartCoroutine(Regen());
        _previosRegen = null;


    }
}