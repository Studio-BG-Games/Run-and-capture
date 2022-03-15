﻿using System;
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
            Color = spawnerColor;
            maxHP = _data.maxHP;
            maxMana = _data.maxMana;
        }
        public override void SetCell(HexCell cell, bool isInstanceTrans = false, bool isPaintingHex = false)
        {
        }

        protected override void CaptureHex()
        {
        }

        protected override void RegenMana()
        {
            _mana += _data.manaRegen;
            UpdateBarCanvas();
        }

        protected override void Attacking()
        {
            Aim(_direction);

            _weapon.Fire(_instance.transform, _direction, this);
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

            if (_hp - dmg > maxHP)
            {
                _hp = maxHP;
            }

            if (_defenceBonus > 0)
            {
                return;
            }


            _hp -= dmg;

            UpdateBarCanvas();
		}
        

        public override bool CanPickUpItem(Item item)
        {
            return false;
        }

        public override void PickUpItem(ItemContainer item)
        {
        }
	}
}