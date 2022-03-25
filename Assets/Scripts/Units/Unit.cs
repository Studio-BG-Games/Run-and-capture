using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using Sirenix.Utilities;
using Units.Views;
using Units.Wariors;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;


namespace Units
{
    public class Unit : UnitBase
    {
        private UnitInfo _unitData;
        public UnitInfo UnitData => _unitData;
        private WariorsData wariorData;
        public bool IsPlayer => _unitData.isPlayer;
        public event Action<ItemContainer> OnItemPickUp;
        public event WariorFactory.SpawnWarior OnShoot;
        private List<UnitBase> NotEnemy;


        public Unit(UnitInfo unitData, Weapon weapon, HexGrid hexGrid, Data.Data data)
        {
            Initialize(weapon, hexGrid);
            _unitData = unitData;
            Color = _unitData.color;
            maxHP = _unitData.maxHP;
            maxMana = _unitData.maxMana;
            InventoryCapacity = _unitData.inventoryCapacity;
            wariorData = data.WariorsData;
        }

        public override void Retreet(HexDirection dir)
        {
            if (!_isCapturing) return;
            var openList = _cell.GetListNeighbours().Where(x => x != null && x.Color == _unitData.color).ToList();
            if (!openList.Contains(_cell.GetNeighbor(dir)))
            {
                return;
            }

            IsBusy = false;
            IsHardToCapture = false;
            BaseView.StopHardCapture();
            Move(dir);
        }

        public override void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction) == null || _cell.GetNeighbor(direction).BuildingInstance != null ||
                IsBusy || IsHardToCapture ||
                (_cell.GetNeighbor(direction).Color != Color
                 && HexManager.UnitCurrentCell.TryGetValue(_cell.GetNeighbor(direction).Color, out var value)
                 && value.cell.Equals(_cell.GetNeighbor(direction)))) return;


            if (_cell.GetNeighbor(direction).Color == _unitData.color ||
                (_cell.GetNeighbor(direction).Color == _easyCaptureColor && _easyCaptureColor != UnitColor.Grey))
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

        protected override void DoTransit(HexDirection direction)
        {
            IsBusy = true;
            _isCapturing = _unitData.color != _cell.GetNeighbor(direction).Color;
            _cell = _cell.GetNeighbor(direction);
            HexManager.UnitCurrentCell[_unitData.color] = (_cell, this);
            RotateUnit(new Vector2((_cell.transform.position - _instance.transform.position).normalized.x,
                (_cell.transform.position - _instance.transform.position).normalized.z));
            _animator.SetTrigger("Move");
            _animator.SetBool("isMoving", IsBusy);
            _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
        }

        public override void SetCell(HexCell cell, bool isInstanceTrans = false, bool isPaintingHex = false)
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

        protected override void CaptureHex()
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
            }

            BaseView.RegenMana();


            UpdateBarCanvas();
            IsBusy = false;
            IsHardToCapture = false;
            _cell.PaintHex(_unitData.color);
        }

        public override void Spawn(HexCoordinates hexCoordinates, HexCell spawnCell = null)
        {
            if (!IsAlive)
            {
                _cell = spawnCell != null ? spawnCell : _hexGrid.GetCellFromCoord(hexCoordinates);
                IsVisible = true;
                _cell.PaintHex(_unitData.color, true);
                _cell.GetListNeighbours().ForEach(x => { x?.PaintHex(Color, true); });
                _inventory = new List<ItemContainer>();
                _inventoryDefence = new List<ItemContainer>();

                HexManager.UnitCurrentCell.Add(_unitData.color,(_cell ,this));

                _instance = Object.Instantiate(_unitData.unitPrefa, _cell.transform.parent);

                _instance.transform.localPosition = _cell.transform.localPosition;

                IsAlive = true;
                _animator = _instance.GetComponent<Animator>();
                BaseView = _instance.AddComponent<UnitView>();


                BaseView.SetUp(_weapon, RegenMana, _unitData.manaRegen, CaptureHex,
                    this, _hexGrid.HardCaptureTime);
                SetAnimLength();
                MusicController.Instance.AddAudioSource(_instance);
                _mana = _unitData.maxMana;
                _hp = _unitData.maxHP;
                SetUpActions();
                _weapon.SetModifiedDamage(0);

                IsBusy = false;
                OnOnPlayerSpawned(this);
            }
        }

        protected override void RegenMana()
        {
            _mana += _unitData.manaRegen;
            UpdateBarCanvas();
        }

        public override bool CanPickUpItem(Item item)
        {
            switch (item.Type)
            {
                case ItemType.ATTACK:
                    if (_inventory.Count < _unitData.inventoryCapacity / 2)
                    {
                        return true;
                    }

                    break;
                case ItemType.DEFENCE:
                    if (_inventoryDefence.Count < _unitData.inventoryCapacity / 2)
                    {
                        return true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public override void PickUpItem(ItemContainer item)
        {
            switch (item.Item.Type)
            {
                case ItemType.ATTACK:
                    if (_inventory.Count < _unitData.inventoryCapacity / 2)
                    {
                        _inventory.Add(item);
                        _cell.Item = null;
                    }

                    break;
                case ItemType.DEFENCE:
                    if (_inventoryDefence.Count < _unitData.inventoryCapacity / 2)
                    {
                        _inventoryDefence.Add(item);
                        _cell.Item = null;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnItemPickUp?.Invoke(item);
        }

        protected override void Attacking()
        {
            Aim(_direction);

            _weapon.Fire(_instance.transform, _direction, this);
            if (IsPlayer)
                OnShoot.Invoke(wariorData.Wariors[1], _unitData.color);
        }

        protected override void UpdateBarCanvas()
        {
            if (_hp > _unitData.maxHP)
                _hp = _unitData.maxHP;
            if (_mana > _unitData.maxMana)
                _mana = _unitData.maxMana;

            float hp = _hp;
            float mana = _mana;
            float maxHp = _unitData.maxHP;
            float maxMana = _unitData.maxMana;
            BarCanvas.ManaBar.DOFillAmount(mana / maxMana, 0.5f).SetEase(Ease.InQuad);
            BarCanvas.HealthBar.DOFillAmount(hp / maxHp, 0.5f).SetEase(Ease.InQuad);
        }

        public override void Death()
        {
            BaseView.OnStep -= MoveEnd;
            BaseView.OnAttackEnd -= AttackEnd;
            BaseView.OnAttack -= Attacking;
            BaseView.OnHit -= Damage;
            IsAlive = false;
            IsBusy = true;
            HexManager.UnitCurrentCell.Remove(Color);
            var hexToPaint = HexManager.CellByColor[Color];
            _animator.SetTrigger("Death");
            var vfx = VFXController.Instance.PlayEffect(HexGrid.Colors[Color].VFXDeathPrefab,
                _instance.transform.position);
            TimerHelper.Instance.StartTimer(() =>
            {
                HexManager.PaintHexList(hexToPaint.Where(x => x.Color == Color).ToList(), UnitColor.Grey);

                Object.Destroy(_instance);
                OnOnDeath(this);
            }, _animLength.Death);

            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(MusicController.Instance.MusicData.SfxMusic.Death, vfx);
            MusicController.Instance.RemoveAudioSource(_instance);
        }

        public override void StartAttack()
        {
            if (IsBusy || !BaseView.Shoot()) return;

            IsBusy = true;
            if (_direction.Equals(Vector2.zero))
            {
                var enemy = AIManager.GetNearestUnit(_weapon.disnatce, this);
                if (enemy == null)
                    _direction =
                        new Vector2(BaseView.transform.forward.x, BaseView.transform.forward.z);
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

        public override HexCell PlaceItemAim(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction).Color != Color)
            {
                BaseView.AimCanvas.SetActive(false);
                return null;
            }

            var cell = _cell.GetNeighbor(direction);
            BaseView.AimCanvas.transform.LookAt(cell.transform);
            return cell;
        }

        protected override void Damage(int dmg)
        {
            if (_defenceBonus == 0 && _hp - dmg <= 0f)
            {
                Death();
            }

            if (_hp - dmg > _unitData.maxHP)
            {
                _hp = _unitData.maxHP;
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