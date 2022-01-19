using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Weapons;
using Random = UnityEngine.Random;


public class UnitView : MonoBehaviour
{
    public Action OnStep;
    public Action OnAttackEnd;
    public Action OnAttack;
    public Action<int> OnHit;
    [SerializeField] private BarCanvas barCanvas;
    [SerializeField] private GameObject aimCanvas;


    private Stack<ShotUIView> _shootUIStack;
    private Stack<ShotUIView> _toReloadStack;
    private Weapon _weapon;
    private int _manaRegen;
    private Action _startRegen;
    private Coroutine _previosRegen;
    private Coroutine _previosReload;
    private Dictionary<string, Action> animActionDic;
    private int _mana;
    private Action _capureHex;
    private Sequence _sequence;
    private AudioSource _audioSource;
    private Unit _unit;
    private float _hardCaptureTime;
    private Action onSupperJump;

    public BarCanvas BarCanvas => barCanvas;
    public GameObject AimCanvas => aimCanvas;
    public UnitColor Color => _unit.Color;
    public int AvailableShots => _shootUIStack.Count;
    

    public Dictionary<string, Action> AnimActionDic => animActionDic;

    public void SetUp(Stack<ShotUIView> shots, Weapon weapon, Action regenMana, int manaRegen, Action captureHex,
        Unit unit, float hardCaptureTime)
    {
        animActionDic = new Dictionary<string, Action> { { "SuperJump", onSupperJump } };
        _shootUIStack = shots;
        _weapon = weapon;
        _toReloadStack = new Stack<ShotUIView>();
        _startRegen = regenMana;
        _manaRegen = manaRegen;
        _capureHex = captureHex;
        _unit = unit;
        _hardCaptureTime = hardCaptureTime;
    }

    public void HardCaptureHex(HexCell cell)
    {
        _unit.IsBusy = true;

        barCanvas.CaptureBack.SetActive(true);
        _sequence = DOTween.Sequence();
        _sequence.Append(_unit.BarCanvas.CaptureBar.DOFillAmount(1f, _hardCaptureTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            _capureHex?.Invoke();
            barCanvas.CaptureBack.SetActive(false);
            MusicController.Instance.PlayRandomClip(MusicController.Instance.MusicData.SfxMusic.Captures,
                cell.gameObject);
        }));
        _sequence.Play();
    }


    public void StopHardCapture()
    {
        _sequence.Kill();
        barCanvas.CaptureBar.DOFillAmount(0f, 0f).SetEase(Ease.Linear);
        _unit.BarCanvas.CaptureBack.SetActive(false);
        
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

    public void RegenMana()
    {
       
        _mana = _unit.Mana;
        StartCoroutine(Regen());
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

    private void AttackEnd() // Методы выполняемые из аниматора
    {
        OnAttackEnd?.Invoke();
    }

    private void Attack()
    {
        OnAttack?.Invoke();
    }

    private void SuperAttack()
    {
        for (var i = 0; i < animActionDic.Count; i++)
        {
            var item = animActionDic.ElementAt(i);
            item.Value?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponView weaponView = other.GetComponent<WeaponView>();
        if (weaponView != null)
        {
            OnHit?.Invoke(weaponView.Weapon.modifiedDamage);
            var vfx = VFXController.Instance.PlayEffect(weaponView.Weapon.VFXGameObject, weaponView.transform.position, weaponView.transform.rotation);
            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(weaponView.Weapon.hitSound, vfx);
            
            other.transform.DOKill();
            Destroy(other.gameObject);
        }

       
    }

    private void OnTriggerStay(Collider other)
    {
        ItemView itemView = other.GetComponent<ItemView>();

        if (itemView != null && _unit.PickUpItem(itemView.Item))
        {
            ItemFabric.Items.Remove(itemView.gameObject);
            Destroy(itemView.gameObject);
        }
    }

    private IEnumerator Reload()
    {
        if (_toReloadStack.Count == 0) yield break; 
        yield return new WaitForSeconds(_weapon.reloadTime);
        if (_toReloadStack.Count == 0) yield break;
        var shot = _toReloadStack.Pop();

        shot.Switch();
        _shootUIStack.Push(shot);

        foreach (var item in _toReloadStack)
        {
            if (Time.deltaTime < _weapon.reloadTime)
            {
                StopCoroutine(_previosReload);
                _previosReload = null;
            }

            _previosReload = StartCoroutine(Reload());
        }
    }

    private IEnumerator Regen()
    {
        if (_mana >= _unit.Data.maxMana)
        {
            yield break;
        }
        
        while (_mana < _unit.Data.maxMana)
        {
            yield return new WaitForSeconds(1f);
            _mana += _manaRegen;
            _startRegen.Invoke();
        }
    }
}