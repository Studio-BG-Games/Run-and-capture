using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AI;
using Chars;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;


namespace Units
{
    public class Unit
    {
        private bool _isAlive;
        private GameObject _instance;
        private List<Item> _inventory;
        private AnimLength _animLength;
        private HexCell _cell;
        private HexGrid _hexGrid;
        public Action<GameObject> onPlayerSpawned;
        private Animator _animator;
        private UnitView _unitView;
        private bool _isBusy;
        private UnitInfo _data;
        private int _hp;
        private int _mana;
        private Weapon _weapon;
        private Vector2 _direction;
        private BarCanvas _barCanvas;
        private bool _isHardToCapture;
        private bool _isCapturing;
        private int _defenceBonus;
        private Camera _camera;

        public int AttackBonus => _weapon.modifiedDamage - _weapon.damage;

        public int DefenceBonus => _defenceBonus;

        public bool IsBusy
        {
            get => _isBusy;
            set => _isBusy = value;
        }

        public UnitView UnitView => _unitView;
        public bool IsAlive => _isAlive;
        public UnitColor Color => _data.color;
        public int InventoryCapacity => _data.inventoryCapacity;
        public Action<Item> OnItemPickUp;
        public Action<Unit> OnDeath;
        public BarCanvas BarCanvas => _barCanvas;
        public GameObject Instance => _instance;
        public UnitInfo Data => _data;
        public int Mana => _mana;
        public int Hp => _hp;
        public List<Item> Inventory => _inventory;
        public Weapon Weapon => _weapon;

        public Unit(UnitInfo unitData, Weapon weapon, HexGrid hexGrid)
        {
            _camera = Camera.main;
            _weapon = weapon;
            _data = unitData;
            _isAlive = false;
            _hexGrid = hexGrid;
            _isBusy = false;
            _isHardToCapture = false;
            _isCapturing = false;
        }

        public void SetUpBonus(float duration, int value, BonusType type)
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
                default:
                    break;
            }
        }


        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction) == null || _isBusy ||
                (_cell.GetNeighbor(direction).Color != Color
                 && HexManager.UnitCurrentCell.TryGetValue(_cell.GetNeighbor(direction).Color, out var value)
                 && value.cell.coordinates.Equals(_cell.GetNeighbor(direction).coordinates))) return;


            _unitView.StopHardCapture();
            if (_cell.GetNeighbor(direction).Color == _data.color)
            {
                DoTransit(direction);
            }
            else if (_cell.GetNeighbor(direction).Color != UnitColor.GREY)
            {
                if (_mana - _hexGrid.HexHardCaptureCost <= 0) return;
                _isHardToCapture = true;
                DoTransit(direction);
            }

            else if (_mana - _hexGrid.HexCaptureCost >= 0)
            {
                if (_mana - _hexGrid.HexHardCaptureCost <= 0) return;
                DoTransit(direction);
            }
        }

        private void DoTransit(HexDirection direction)
        {
            _isBusy = true;
            _isCapturing = _data.color != _cell.GetNeighbor(direction).Color;
            _cell = _cell.GetNeighbor(direction);
            HexManager.UnitCurrentCell[_data.color] = (_cell, this);
            RotateUnit(new Vector2((_cell.transform.position - _instance.transform.position).normalized.x,
                (_cell.transform.position - _instance.transform.position).normalized.z));
            _animator.SetTrigger("Move");
            _animator.SetBool("isMoving", _isBusy);
            _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
        }

        private void CaptureHex()
        {
            if (_isHardToCapture)
            {
                _mana -= _hexGrid.HexHardCaptureCost;
            }
            else
            {
                _mana -= _hexGrid.HexCaptureCost;
            }

            UnitView.RegenMana();

            UpdateBarCanvas();
            _isBusy = false;
            _isHardToCapture = false;
            _cell.PaintHex(_data.color);
        }

        private void SetAnimLength()
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
                    default:
                        break;
                }
            }
        }

        public void Spawn(HexCoordinates hexCoordinates)
        {
            if (!_isAlive)
            {
                _cell = _hexGrid.GetCellFromCoord(hexCoordinates);
                _cell.PaintHex(_data.color);
                _cell.GetListNeighbours().ForEach(x => x?.PaintHex(Color));
                _inventory = new List<Item>();


                HexManager.UnitCurrentCell.Add(_data.color, (_cell, this));

                _instance = Object.Instantiate(_data.unitPrefa, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;

                _isAlive = true;
                _animator = _instance.GetComponent<Animator>();
                _unitView = _instance.GetComponent<UnitView>();
                _barCanvas = _unitView.BarCanvas;
                _unitView.SetUp(_barCanvas.SpawnShotUI(_weapon.shots), _weapon, RegenMana, _data.manaRegen, CaptureHex,
                    this, _hexGrid.HardCaptureTime);
                SetAnimLength();
                MusicController.Instance.AddAudioSource(_instance);
                _mana = _data.maxMana;
                _hp = _data.maxHP;
                SetUpActions();
                _weapon.SetModifiedDamage(0);
                BarCanvas.transform.LookAt(
                    BarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                    _camera.transform.rotation * Vector3.up);
                _isBusy = false;
                onPlayerSpawned?.Invoke(_instance);
            }
        }

        private void RegenMana()
        {
            _mana += _data.manaRegen;
            UpdateBarCanvas();
        }

        public bool PickUpItem(Item item)
        {
            if (_inventory.Count < _data.inventoryCapacity)
            {
                item.PickUp(this);
                _inventory.Add(item);
                OnItemPickUp?.Invoke(item);
                return true;
            }

            return false;
        }

        public void UseItem(Item item)
        {
            _inventory.Remove(item);
        }

        private void MoveEnd()
        {
            _isBusy = false;
            _animator.SetBool("isMoving", _isBusy);

            if (!_isCapturing)
            {
                return;
            }

            if (_isHardToCapture)
            {
                _unitView.HardCaptureHex(_cell);
            }
            else
            {
                CaptureHex();
            }

            _isHardToCapture = false;
        }

        private void AttackEnd()
        {
            _isBusy = false;
            UpdateBarCanvas();
        }

        private void Attacking()
        {
            Aim(_direction);


            _weapon.Fire(_instance.transform, _direction);
        }

        private void SetUpActions()
        {
            _unitView.OnStep += MoveEnd;
            _unitView.OnAttackEnd += AttackEnd;
            _unitView.OnAttack += Attacking;
            _unitView.OnHit += Damage;
        }

        private void UpdateBarCanvas()
        {
            if (_hp > _data.maxHP)
                _hp = _data.maxHP;
            if (_mana > _data.maxMana)
                _mana = _data.maxMana;

            float hp = _hp;
            float mana = _mana;
            float maxHp = _data.maxHP;
            float maxMana = _data.maxMana;
            _barCanvas.ManaBar.DOFillAmount(mana / maxMana, 0.5f).SetEase(Ease.InQuad);
            _barCanvas.HealthBar.DOFillAmount(hp / maxHp, 0.5f).SetEase(Ease.InQuad);
        }

        public void Death()
        {
            _unitView.OnStep -= MoveEnd;
            _unitView.OnAttackEnd -= AttackEnd;
            _unitView.OnAttack -= Attacking;
            _unitView.OnHit -= Damage;
            _isAlive = false;
            _isBusy = true;
            HexManager.UnitCurrentCell.Remove(Color);
            _animator.SetTrigger("Death");
            var vfx = VFXController.Instance.PlayEffect(HexGrid.Colors[Color].VFXDeathPrefab,
                _instance.transform.position);
            TimerHelper.Instance.StartTimer(() =>
            {
                Object.Destroy(_instance);
                OnDeath?.Invoke(this);
            }, _animLength.Death);

            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(MusicController.Instance.MusicData.SfxMusic.Death, vfx);
            MusicController.Instance.RemoveAudioSource(_instance);
        }


        public void StartAttack()
        {
            if (_isBusy || !_unitView.Shoot()) return;

            _isBusy = true;
            if (_direction.Equals(Vector2.zero))
            {
                var enemy = AIManager.GetNearestUnit(_weapon.disnatce, this);
                if (enemy == null)
                    _direction =
                        new Vector2(_unitView.transform.forward.x, _unitView.transform.forward.z);
                else
                {
                    var dir = DirectionHelper.DirectionTo(_instance.transform.position,
                        enemy.Instance.transform.position);
                    _direction = new Vector2(dir.x, dir.z);
                }
            }

            RotateUnit(_direction);
            _animator.SetTrigger("Attack");
        }

        private void RotateUnit(Vector2 direction)
        {
            _unitView.transform.DOLookAt(new Vector3(direction.x, 0, direction.y) + _unitView.transform.position,
                0.1f).onUpdate += () => BarCanvas.transform.LookAt(
                BarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                _camera.transform.rotation * Vector3.up);
        }

        public void Aim(Vector2 direction)
        {
            _unitView.AimCanvas.transform.LookAt(
                new Vector3(direction.x, 0, direction.y) + _unitView.transform.position);
            _direction = direction;
        }

        public HexCell PlaceItemAim(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction).Color != Color)
            {
                _unitView.AimCanvas.SetActive(false);
                return null;
            }

            var cell = _cell.GetNeighbor(direction);
            _unitView.AimCanvas.transform.LookAt(cell.transform);
            return cell;
        }

        private void Damage(int dmg)
        {
            if (_defenceBonus == 0 && _hp - dmg <= 0f)
            {
                Death();
            }

            if (_defenceBonus > 0)
            {
                _defenceBonus -= dmg;
            }

            else
            {
                SetUpBonus(0, 0, BonusType.Defence);
                _hp -= dmg;
            }

            UpdateBarCanvas();
        }
    }
}