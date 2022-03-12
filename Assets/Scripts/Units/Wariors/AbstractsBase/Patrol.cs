using System;
using System.Linq;
using AI;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using UnityEngine;
using Weapons;

namespace Units.Wariors.AbstractsBase
{
    public abstract class Patrol : UnitBase
    {
        protected WariorInfo _data;
        public WariorInfo Data => _data;


        public Patrol(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor)
        {
            Initialize(weapon, hexGrid);
            _data = data;
            _data.color = spawnerColor;
            Color = _data.color;
            maxHP = _data.maxHP;
            maxMana = _data.maxMana;
        }

        public override void Retreet(HexDirection dir)
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

        public override void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction) == null || _cell.GetNeighbor(direction).BuildingInstance != null ||
                IsBusy || IsHardToCapture ||
                (_cell.GetNeighbor(direction).Color != Color
                 && HexManager.UnitCurrentCell.TryGetValue(_cell.GetNeighbor(direction).Color, out var value)
                 && value.cell.Equals(_cell.GetNeighbor(direction)))) return;


            if (_cell.GetNeighbor(direction).Color == _data.color ||
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

            UnitView.RegenMana();


            UpdateBarCanvas();
            IsBusy = false;
            IsHardToCapture = false;
            _cell.PaintHex(_data.color);
        }
        

        protected override void RegenMana()
        {
            _mana += _data.manaRegen;
            UpdateBarCanvas();
        }

        public override bool CanPickUpItem(Item item)
        {
            return false;
        }

        public override void PickUpItem(ItemContainer item)
        {
            return;
        }

        protected override void UpdateBarCanvas()
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

        public override void StartAttack()
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

        public override HexCell PlaceItemAim(HexDirection direction)
        {
            return null;
        }

        protected override void Damage(int dmg)
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