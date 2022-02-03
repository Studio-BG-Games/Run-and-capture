using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using Units;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;


public class UnitView : MonoBehaviour
{
    public event Action OnStep;
    public event Action OnAttackEnd;
    public event Action OnAttack;
    public Action<int> OnHit;
    private BarCanvas _barCanvas;
    private GameObject _aimCanvas;


    private Stack<ShotUIView> _shootUIStack;
    private Stack<ShotUIView> _toReloadStack;
    private Weapon _weapon;
    private int _manaRegen;
    private Action _startRegen;
    private Coroutine _previosRegen;
    private Coroutine _previosReload;
    private Dictionary<string, Action> animActionDic;
    private int _mana;
    private event Action CaptureHex;
    private Sequence _sequence;
    private AudioSource _audioSource;
    private Unit _unit;
    private float _hardCaptureTime;
    private Action onSupperJump;
    private Coroutine _previousRegenCoroutine;

    public BarCanvas BarCanvas => _barCanvas;
    public GameObject AimCanvas => _aimCanvas;
    public UnitColor Color => _unit.Color;
    public int AvailableShots => _shootUIStack.Count;
    


    public Dictionary<string, Action> AnimActionDic => animActionDic;

    public void SetBar(BarCanvas barCanvas, GameObject aimCanvas)
    {
        _barCanvas = Instantiate(barCanvas, _unit.Instance.transform);
        _aimCanvas = Instantiate(aimCanvas, _unit.Instance.transform);
        _shootUIStack = _barCanvas.SpawnShotUI(_weapon.shots);
        _aimCanvas.SetActive(false);
        _barCanvas.transform.LookAt(
            BarCanvas.transform.position + Camera.main.transform.rotation * Vector3.back,
            Camera.main.transform.rotation * Vector3.up);
    }

    public void SetUp(Weapon weapon, Action regenMana, int manaRegen, Action captureHex,
        Unit unit, float hardCaptureTime)
    {
        animActionDic = new Dictionary<string, Action> { { "SuperJump", onSupperJump } };
        
        _weapon = weapon;
        _toReloadStack = new Stack<ShotUIView>();
        _startRegen = regenMana;
        _manaRegen = manaRegen;
        CaptureHex = captureHex;
        _unit = unit;
        _hardCaptureTime = hardCaptureTime;
    }

    public void HardCaptureHex(HexCell cell)
    {
        

        _barCanvas.CaptureBack.SetActive(true);
        _sequence = DOTween.Sequence();
        _sequence.Append(_unit.BarCanvas.CaptureBar.DOFillAmount(1f, _hardCaptureTime).SetEase(Ease.Linear).OnComplete(
            () =>
            {
                CaptureHex?.Invoke();
                _barCanvas.CaptureBack.SetActive(false);
                MusicController.Instance.PlayRandomClip(MusicController.Instance.MusicData.SfxMusic.Captures,
                    cell.gameObject);
            }));
        _sequence.Play();
    }


    public void StopHardCapture()
    {
        _sequence.Kill();
        
        _barCanvas.CaptureBar.DOFillAmount(0f, 0f).SetEase(Ease.Linear);
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
        if (_previosRegen != null)
        {
            StopCoroutine(_previosRegen);
        }

        _previosRegen = StartCoroutine(Regen());
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
            var vfx = VFXController.Instance.PlayEffect(weaponView.Weapon.VFXGameObject, weaponView.transform.position,
                weaponView.transform.rotation);
            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(weaponView.Weapon.hitSound, vfx);

            other.transform.DOKill();
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        ItemView itemView = other.GetComponent<ItemView>();
        
        if (itemView == null || itemView.pickedUp || !_unit.CanPickUpItem(itemView.Item)) return;
        itemView.pickedUp = true;
        itemView.Item.PickUp(_unit);
        ItemFabric.Items.Remove(itemView.gameObject);
        
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