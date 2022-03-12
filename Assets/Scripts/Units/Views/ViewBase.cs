using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using Items.ItemViews;
using Sirenix.Utilities;
using UnityEngine;
using Weapons;

namespace Units.Views
{
    public abstract class ViewBase : MonoBehaviour
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
        protected int _manaRegen;
        protected Action _startRegen;
        private Coroutine _previosRegen;
        private Coroutine _previosReload;
        private Dictionary<string, Action> animActionDic;
        protected int _mana;
        private event Action CaptureHex;
        private AudioSource _audioSource;
        protected UnitBase _unit;
        private float _hardCaptureTime;
        private Action onSupperJump;
        private Coroutine _previousRegenCoroutine;
        private List<Material> _materials;

        public BarCanvas BarCanvas => _barCanvas;
        public GameObject AimCanvas => _aimCanvas;
        public int AvailableShots => _shootUIStack.Count;

        public UnitBase Unit => _unit;

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
            UnitBase unit, float hardCaptureTime)
        {
            animActionDic = new Dictionary<string, Action> { { "SuperJump", onSupperJump } };
            _materials = new List<Material>();
            _weapon = weapon;
            _toReloadStack = new Stack<ShotUIView>();
            _startRegen = regenMana;
            _manaRegen = manaRegen;
            CaptureHex = captureHex;
            _unit = unit;
            _hardCaptureTime = hardCaptureTime;
        }

        public void SetInvisible(bool isVisible)
        {
            var i = 0;
            transform.GetChilds().ForEach(x =>
            {
                if (x.gameObject.TryGetComponent(typeof(SkinnedMeshRenderer), out var mesh))
                {
                    if (!isVisible)
                    {
                        var materialColor = ((SkinnedMeshRenderer)mesh).material.color;
                        materialColor.a=0;
                        ((SkinnedMeshRenderer) mesh).material.color = materialColor;
                    }
                    else
                    {
                        var materialColor = ((SkinnedMeshRenderer)mesh).material.color;
                        materialColor.a=100;
                        ((SkinnedMeshRenderer) mesh).material.color = materialColor;
                    }
                }

                if (x.gameObject.TryGetComponent(typeof(MeshRenderer), out var mesh1))
                {
                    if (!isVisible)
                    {
                        var materialColor = ((SkinnedMeshRenderer)mesh1).material.color;
                        materialColor.a=0;
                        ((SkinnedMeshRenderer) mesh1).material.color = materialColor;
                    }
                    else
                    {
                        var materialColor = ((SkinnedMeshRenderer)mesh1).material.color;
                        materialColor.a=100;
                        ((SkinnedMeshRenderer) mesh1).material.color = materialColor;
                    }
                }
            });
        }


        public virtual void HardCaptureHex(HexCell cell)
        {
            _unit.BarCanvas.CaptureBar.DOFillAmount(0f, 0);
            _barCanvas.CaptureBack.SetActive(true);

            _unit.BarCanvas.CaptureBar.DOFillAmount(1f, _hardCaptureTime).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    CaptureHex?.Invoke();
                    _barCanvas.CaptureBack.SetActive(false);
                    MusicController.Instance.PlayRandomClip(MusicController.Instance.MusicData.SfxMusic.Captures,
                        cell.gameObject);
                });
        }


        public virtual void StopHardCapture()
        {
            _barCanvas.CaptureBar.DOKill();

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

        protected abstract void OnTriggerEnter(Collider other);

        private void OnTriggerStay(Collider other)
        {
            ItemView itemView = other.GetComponent<ItemView>();

            if (itemView == null || itemView.pickedUp || !_unit.CanPickUpItem(itemView.Item)) return;
            itemView.pickedUp = true;
            itemView.PickUp(Unit);

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

        protected abstract IEnumerator Regen();
    }
}