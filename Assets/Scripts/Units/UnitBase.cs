using System;
using System.Collections.Generic;
using System.Linq;
using Chars;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using Units.Views;
using UnityEngine;
using Weapons;

namespace Units
{
    public abstract class UnitBase
    {
        public event Action<UnitBase> OnSpawned;
        public event Action<UnitBase> OnDeath;
        protected GameObject _instance;
        protected List<ItemContainer> _inventory;
        protected List<ItemContainer> _inventoryDefence;
        protected AnimLength _animLength;
        protected HexCell _cell;
        protected HexGrid _hexGrid;
        protected Animator _animator;
        protected int _hp;
        protected int _mana;
        public int maxMana { protected set; get; }
        public int maxHP { protected set; get; }
        public int InventoryCapacity { protected set; get; }


        protected Weapon _weapon;
        protected Vector2 _direction;

        protected bool _isCapturing;
        protected bool _isInfiniteMana;

        protected int _defenceBonus;
        private Camera _camera;
        protected UnitColor _easyCaptureColor;
        public UnitColor Color { get; protected set; }
        
        public bool IsStaned;
        public bool isSwitched;
        public int AttackBonus => _weapon.modifiedDamage - _weapon.damage;

        
        public int DefenceBonus => _defenceBonus;

        public bool IsBusy { get; set; }
        
        public bool IsAlive { get; protected set; }

        public bool IsHardToCapture { get; protected set; }



        public BarCanvas BarCanvas => BaseView.BarCanvas;
        public GameObject Instance => _instance;
        public int Mana => _mana;
        public ViewBase BaseView { get; protected set; }

        public int Hp => _hp;
        public List<ItemContainer> Inventory => _inventory;
        public List<ItemContainer> InventoryDefence => _inventoryDefence;
        public Weapon Weapon => _weapon;
        public Animator Animator => _animator;

        public bool IsVisible
        {
            get;
            protected set;
        }

        public virtual void Initialize(Weapon weapon, HexGrid hexGrid)
        {
            _camera = Camera.main;
            _weapon = weapon;
            IsAlive = false;
            _hexGrid = hexGrid;
            IsBusy = false;
            IsHardToCapture = false;
            _isCapturing = false;
            _easyCaptureColor = UnitColor.Grey;
        }

        public virtual void SetUpBonus(float duration, int value, BonusType type)
        {
            switch (type)
            {
                case BonusType.Attack:
                    TimerHelper.Instance.StartTimer(() => _weapon.SetModifiedDamage(0), duration);
                    _weapon.SetModifiedDamage(value);
                    break;
                case BonusType.Defence:
                    TimerHelper.Instance.StartTimer(() => _defenceBonus = 0, duration);
                    _defenceBonus = value;
                    break;
                case BonusType.Heal:
                    break;
                case BonusType.Magnet:
                    var col = BaseView.gameObject.GetComponent<CapsuleCollider>();
                    var defRadius = col.radius;
                    col.radius = value * HexGrid.HexDistance;
                    TimerHelper.Instance.StartTimer(() => col.radius = defRadius, duration);
                    break;
                case BonusType.Mana:
                    _isInfiniteMana = true;
                    TimerHelper.Instance.StartTimer(() =>  _isInfiniteMana = false, duration);
                    break;
                case BonusType.Invisible:
                    IsVisible = false;
                    BaseView.SetInvisible(IsVisible);
                    TimerHelper.Instance.StartTimer(() =>
                    {
                        IsVisible = true;
                        BaseView.SetInvisible(IsVisible);
                    }, duration);
                    break;
                default:
                    break;
            }
        }

        public void SetTimeScale(float scale)
        {
            _animator.speed = scale;
           
        }

        public abstract void Retreet(HexDirection dir);

        public abstract void Move(HexDirection direction);

        protected abstract void DoTransit(HexDirection direction);

        public abstract void SetCell(HexCell cell, bool isInstanceTrans = false, bool isPaintingHex = false);

        public void SetEasyColor(UnitColor color, float time)
        {
            _easyCaptureColor = color;
            if (time > 0f)
            {
                TimerHelper.Instance.StartTimer(() => _easyCaptureColor = UnitColor.Grey, time);
            }
           
        }

        protected abstract void CaptureHex();

        protected void SetAnimLength()
        {
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                switch (clip.name)
                {
                    case "MoveJump":
                        _animLength.Move = clip.length;
                        break;
                    case "Attack":
                        _animLength.Attack = clip.length;
                        break;
                    case "Dead":
                        _animLength.Death = clip.length;
                        break;
                    case "SuperAttack":
                        _animLength.SuperJump = clip.length;
                        break;
                    default:
                        break;
                }
            }
        }

        public abstract void Spawn(HexCoordinates hexCoordinates, HexCell spawnCell = null);

        protected abstract void RegenMana();

        public abstract bool CanPickUpItem(Item item);

        public abstract void PickUpItem(ItemContainer item);

        public void UseItem(Item item)
        {
            if (item.Type == ItemType.ATTACK)
            {
                var i = _inventory.First(i => i.Item == item);
                _inventory.Remove(i);
            }
            else
            {
                var i = _inventoryDefence.First(i => i.Item == item);
                _inventoryDefence.Remove(i);
            }
        }

        protected void MoveEnd()
        {
            IsBusy = false;
            _animator.SetBool("isMoving", IsBusy);

            if (!_isCapturing)
            {
                return;
            }

            if (IsHardToCapture)
            {
                BaseView.HardCaptureHex(_cell);
            }
            else
            {
                CaptureHex();
            }
        }

        protected void AttackEnd()
        {
            IsBusy = false;
            UpdateBarCanvas();
        }

        protected abstract void Attacking();

        protected void SetUpActions()
        {
            BaseView.OnStep += MoveEnd;
            BaseView.OnAttackEnd += AttackEnd;
            BaseView.OnAttack += Attacking;
            BaseView.OnHit += Damage;
        }

        protected abstract void UpdateBarCanvas();

        public abstract void Death();


        public abstract void StartAttack();

        public void RotateUnit(Vector2 direction)
        {
            BaseView.transform.DOLookAt(new Vector3(direction.x, 0, direction.y) + BaseView.transform.position,
                0.1f).onUpdate += () => BarCanvas.transform.LookAt(
                BarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                _camera.transform.rotation * Vector3.up);
        }

        public void Aim(Vector2 direction)
        {
            BaseView.AimCanvas.transform.LookAt(
                new Vector3(direction.x, 0, direction.y) + BaseView.transform.position);
            _direction = direction;
        }

        public abstract HexCell PlaceItemAim(HexDirection direction);

        protected abstract void Damage(int dmg);

        protected virtual void OnOnPlayerSpawned(UnitBase obj)
        {
            OnSpawned?.Invoke(obj);
        }

        protected virtual void OnOnDeath(UnitBase obj)
        {
            OnDeath?.Invoke(obj);
        }
    }
}