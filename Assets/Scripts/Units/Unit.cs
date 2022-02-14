using System;
using System.Collections.Generic;
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
        private GameObject _instance;
        private List<Item> _inventory;
        private List<Item> _inventoryDefence;
        private AnimLength _animLength;
        private HexCell _cell;
        private HexGrid _hexGrid;
        public event Action<Unit> OnPlayerSpawned;
        private Animator _animator;
        private UnitInfo _data;
        private int _hp;
        private int _mana;
        private Weapon _weapon;
        private Vector2 _direction;

        private bool _isCapturing;
        private bool _isInfiniteMana;
       
        private int _defenceBonus;
        private Camera _camera;
        private UnitColor _easyCaptureColor;
        
        public bool IsStaned;
        public int AttackBonus => _weapon.modifiedDamage - _weapon.damage;

        
        public int DefenceBonus => _defenceBonus;

        public bool IsBusy { get; set; }

        public UnitView UnitView { get; private set; }

        public bool IsAlive { get; private set; }

        public bool IsHardToCapture { get; private set; }


        public UnitColor Color => _data.color;
        public int InventoryCapacity => _data.inventoryCapacity;
        public event Action<Item> OnItemPickUp;
        public event Action<Unit> OnDeath;
        public BarCanvas BarCanvas => UnitView.BarCanvas;
        public GameObject Instance => _instance;
        public UnitInfo Data => _data;
        public int Mana => _mana;
        public int Hp => _hp;
        public List<Item> Inventory => _inventory;
        public List<Item> InventoryDefence => _inventoryDefence;
        public Weapon Weapon => _weapon;
        public bool IsPlayer => _data.isPlayer;
        public Animator Animator => _animator;


        public Unit(UnitInfo unitData, Weapon weapon, HexGrid hexGrid)
        {
            _camera = Camera.main;
            _weapon = weapon;
            _data = unitData;
            IsAlive = false;
            _hexGrid = hexGrid;
            IsBusy = false;
            IsHardToCapture = false;
            _isCapturing = false;
            _easyCaptureColor = UnitColor.Grey;
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
                case BonusType.Heal:
                    break;
                case BonusType.Magnet:
                    var col = UnitView.gameObject.GetComponent<CapsuleCollider>();
                    var defRadius = col.radius;
                    col.radius = value * HexGrid.HexDistance;
                    TimerHelper.Instance.StartTimer(() => col.radius = defRadius, duration);
                    break;
                case BonusType.Mana:
                    _isInfiniteMana = true;
                    TimerHelper.Instance.StartTimer(() =>  _isInfiniteMana = false, duration);
                    break;
                default:
                    break;
            }
        }

        public void Retreet(HexDirection dir)
        {
            if (!_isCapturing) return;
            var openList = _cell.GetListNeighbours().Where(x => x != null && x.Color == _data.color).ToList();
            if (!openList.Contains(_cell.GetNeighbor(dir)))
            {
                return;
            }

            IsBusy = false;
            IsHardToCapture = false;
            UnitView.StopHardCapture();
            Move(dir);
        }

        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction) == null || _cell.GetNeighbor(direction).BuildingInstance != null ||
                IsBusy || IsHardToCapture ||
                (_cell.GetNeighbor(direction).Color != Color
                 && HexManager.UnitCurrentCell.TryGetValue(_cell.GetNeighbor(direction).Color, out var value)
                 && value.cell.Equals(_cell.GetNeighbor(direction)))) return;


            if (_cell.GetNeighbor(direction).Color == _data.color || _cell.GetNeighbor(direction).Color == _easyCaptureColor)
            {
                DoTransit(direction);
            }
            else if (_cell.GetNeighbor(direction).Color != UnitColor.Grey)
            {
                if (_mana - _hexGrid.HexHardCaptureCost <= 0) return;
                IsHardToCapture = true;
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
            IsBusy = true;
            _isCapturing = _data.color != _cell.GetNeighbor(direction).Color;
            _cell = _cell.GetNeighbor(direction);
            HexManager.UnitCurrentCell[_data.color] = (_cell, this);
            RotateUnit(new Vector2((_cell.transform.position - _instance.transform.position).normalized.x,
                (_cell.transform.position - _instance.transform.position).normalized.z));
            _animator.SetTrigger("Move");
            _animator.SetBool("isMoving", IsBusy);
            _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
        }

        public void SetCell(HexCell cell, bool isInstanceTrans = false, bool isPaintingHex = false)
        {
            _cell = cell;
            HexManager.UnitCurrentCell[Color] = (cell, this);
            if (!isInstanceTrans)
            {
                IsBusy = true;
                _instance.transform.DOMove(_cell.transform.position, _animLength.SuperJump)
                    .OnComplete(() => IsBusy = false);
            }
            else
            {
                _instance.transform.DOMove(_cell.transform.position, 0.5f).SetEase(Ease.Linear);
            }

            if (isPaintingHex)
            {
                cell.PaintHex(Color, true);
            }

            
        }

        public void SetEasyColor(UnitColor color, float time)
        {
            _easyCaptureColor = color;
            if (time > 0f)
            {
                TimerHelper.Instance.StartTimer(() => _easyCaptureColor = UnitColor.Grey, time);
            }
           
        }

        private void CaptureHex()
        {
            if (!_isInfiniteMana)
            {
                if (IsHardToCapture)
                {
                    _mana -= _hexGrid.HexHardCaptureCost;
                }
                else
                {
                    _mana -= _hexGrid.HexCaptureCost;
                }

                UnitView.RegenMana();
            }

            UpdateBarCanvas();
            IsBusy = false;
            IsHardToCapture = false;
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
                    case "Super_Attack":
                        _animLength.SuperJump = clip.length;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Spawn(HexCoordinates hexCoordinates, HexCell spawnCell = null)
        {
            if (!IsAlive)
            {
                _cell = spawnCell != null ? spawnCell : _hexGrid.GetCellFromCoord(hexCoordinates);

                _cell.PaintHex(_data.color, true);
                _cell.GetListNeighbours().ForEach(x => { x?.PaintHex(Color, true); });
                _inventory = new List<Item>();
                _inventoryDefence = new List<Item>();

                HexManager.UnitCurrentCell.Add(_data.color, (_cell, this));

                _instance = Object.Instantiate(_data.unitPrefa, _cell.transform.parent);

                _instance.transform.localPosition = _cell.transform.localPosition;

                IsAlive = true;
                _animator = _instance.GetComponent<Animator>();
                UnitView = _instance.AddComponent<UnitView>();


                UnitView.SetUp(_weapon, RegenMana, _data.manaRegen, CaptureHex,
                    this, _hexGrid.HardCaptureTime);
                SetAnimLength();
                MusicController.Instance.AddAudioSource(_instance);
                _mana = _data.maxMana;
                _hp = _data.maxHP;
                SetUpActions();
                _weapon.SetModifiedDamage(0);

                IsBusy = false;
                OnPlayerSpawned?.Invoke(this);
            }
        }

        private void RegenMana()
        {
            _mana += _data.manaRegen;
            UpdateBarCanvas();
        }

        public bool CanPickUpItem(Item item)
        {
            switch (item.Type)
            {
                case ItemType.ATTACK:
                    if (_inventory.Count < _data.inventoryCapacity / 2)
                    {
                        return true;
                    }

                    break;
                case ItemType.DEFENCE:
                    if (_inventoryDefence.Count < _data.inventoryCapacity / 2)
                    {
                        return true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public void PickUpItem(Item item)
        {
            switch (item.Type)
            {
                case ItemType.ATTACK:
                    if (_inventory.Count < _data.inventoryCapacity / 2)
                    {
                        _inventory.Add(item);
                        OnItemPickUp?.Invoke(item);
                        _cell.Item = null;
                    }

                    break;
                case ItemType.DEFENCE:
                    if (_inventoryDefence.Count < _data.inventoryCapacity / 2)
                    {
                        _inventoryDefence.Add(item);
                        OnItemPickUp?.Invoke(item);
                        _cell.Item = null;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UseItem(Item item)
        {
            if (item.Type == ItemType.ATTACK)
                _inventory.Remove(item);
            else
            {
                _inventoryDefence.Remove(item);
            }
        }

        private void MoveEnd()
        {
            IsBusy = false;
            _animator.SetBool("isMoving", IsBusy);

            if (!_isCapturing)
            {
                return;
            }

            if (IsHardToCapture)
            {
                UnitView.HardCaptureHex(_cell);
            }
            else
            {
                CaptureHex();
            }
        }

        private void AttackEnd()
        {
            IsBusy = false;
            UpdateBarCanvas();
        }

        private void Attacking()
        {
            Aim(_direction);


            _weapon.Fire(_instance.transform, _direction, this);
        }

        private void SetUpActions()
        {
            UnitView.OnStep += MoveEnd;
            UnitView.OnAttackEnd += AttackEnd;
            UnitView.OnAttack += Attacking;
            UnitView.OnHit += Damage;
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
            BarCanvas.ManaBar.DOFillAmount(mana / maxMana, 0.5f).SetEase(Ease.InQuad);
            BarCanvas.HealthBar.DOFillAmount(hp / maxHp, 0.5f).SetEase(Ease.InQuad);
        }

        public void Death()
        {
            UnitView.OnStep -= MoveEnd;
            UnitView.OnAttackEnd -= AttackEnd;
            UnitView.OnAttack -= Attacking;
            UnitView.OnHit -= Damage;
            IsAlive = false;
            IsBusy = true;
            HexManager.UnitCurrentCell.Remove(Color);
            var hexToPaint = HexManager.CellByColor[Color];
            _animator.SetTrigger("Death");
            var vfx = VFXController.Instance.PlayEffect(HexGrid.Colors[Color].VFXDeathPrefab,
                _instance.transform.position);
            TimerHelper.Instance.StartTimer(() =>
            {
                HexManager.PaintHexList(hexToPaint, UnitColor.Grey);

                Object.Destroy(_instance);
                OnDeath?.Invoke(this);
            }, _animLength.Death);
            _inventory.ForEach(x => x.Dispose());
            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(MusicController.Instance.MusicData.SfxMusic.Death, vfx);
            MusicController.Instance.RemoveAudioSource(_instance);
        }


        public void StartAttack()
        {
            if (IsBusy || !UnitView.Shoot()) return;

            IsBusy = true;
            if (_direction.Equals(Vector2.zero))
            {
                var enemy = AIManager.GetNearestUnit(_weapon.disnatce, this);
                if (enemy == null)
                    _direction =
                        new Vector2(UnitView.transform.forward.x, UnitView.transform.forward.z);
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

        public void RotateUnit(Vector2 direction)
        {
            UnitView.transform.DOLookAt(new Vector3(direction.x, 0, direction.y) + UnitView.transform.position,
                0.1f).onUpdate += () => BarCanvas.transform.LookAt(
                BarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                _camera.transform.rotation * Vector3.up);
        }

        public void Aim(Vector2 direction)
        {
            UnitView.AimCanvas.transform.LookAt(
                new Vector3(direction.x, 0, direction.y) + UnitView.transform.position);
            _direction = direction;
        }

        public HexCell PlaceItemAim(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction).Color != Color)
            {
                UnitView.AimCanvas.SetActive(false);
                return null;
            }

            var cell = _cell.GetNeighbor(direction);
            UnitView.AimCanvas.transform.LookAt(cell.transform);
            return cell;
        }

        private void Damage(int dmg)
        {
            if (_defenceBonus == 0 && _hp - dmg <= 0f)
            {
                Death();
            }

            if (_hp - dmg > _data.maxHP)
            {
                _hp = _data.maxHP;
            }

            if (_defenceBonus > 0)
            {
                return;
            }


            _hp -= dmg;

            UpdateBarCanvas();
        }
    }
}