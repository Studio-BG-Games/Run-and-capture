﻿using System.Collections.Generic;
using System.Linq;
using AI;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using Units.Views;
using Units.Wariors.AbstractsBase;
using UnityEngine;
using Weapons;

namespace Units.Wariors
{
    public class Holem : Patrol
    {
        public Holem(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor) : base(data, weapon, hexGrid, spawnerColor)
        {
        }

        public override void Retreet(HexDirection dir)
        {
            if (!_isCapturing) return;
            var openList = _cell.GetListNeighbours().Where(x => x != null && x.Color == Color).ToList();
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
				 
            if (_cell.GetNeighbor(direction).Color != Color ) return;
                DoTransit(direction);
            
        }

        protected override void DoTransit(HexDirection direction)
        {
            IsBusy = true;
            _isCapturing = Color != _cell.GetNeighbor(direction).Color;
            _cell = _cell.GetNeighbor(direction);
            HexManager.UnitCurrentCell[Color] = (_cell, this);
            RotateUnit(new Vector2((_cell.transform.position - _instance.transform.position).normalized.x,
                (_cell.transform.position - _instance.transform.position).normalized.z));
            _animator.SetTrigger("Move");
            _animator.SetBool("isMoving", IsBusy);
            _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
        }



        public override void Spawn(HexCoordinates hexCoordinates, HexCell spawnCell = null)
        {
            if (!IsAlive)
            {
                _cell = spawnCell != null ? spawnCell : _hexGrid.GetCellFromCoord(hexCoordinates);
                IsVisible = true;
                _inventory = new List<ItemContainer>();
                _inventoryDefence = new List<ItemContainer>();

                _instance = Object.Instantiate(_data.wariorPrefab, _cell.transform.parent);

                _instance.transform.localPosition = _cell.transform.localPosition;

                IsAlive = true;
                _animator = _instance.GetComponent<Animator>();
                BaseView = _instance.AddComponent<WariorView>();


                BaseView.SetUp(_weapon, RegenMana, _data.manaRegen, CaptureHex,
                    this, _hexGrid.HardCaptureTime);
                SetAnimLength();
                MusicController.Instance.AddAudioSource(_instance);
                _mana = maxMana;
                _hp = maxHP;
                SetUpActions();
                _weapon.SetModifiedDamage(0);

                IsBusy = false;
                OnOnPlayerSpawned(this);
            }
        }



        protected override void UpdateBarCanvas()
        {
            if (_hp > maxHP)
                _hp =maxHP;
            if (_mana > this.maxMana)
                _mana = this.maxMana;

            float hp = _hp;
            float mana = _mana;
            float maxHp = maxHP;
            float maxMana = this.maxMana;
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
            _animator.SetTrigger("Death");
            var vfx = VFXController.Instance.PlayEffect(HexGrid.Colors[Color].VFXDeathPrefab,
                _instance.transform.position);
            TimerHelper.Instance.StartTimer(() =>
            {
                Object.Destroy(_instance);
                OnOnDeath(this);
            }, _animLength.Death);

            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(MusicController.Instance.MusicData.SfxMusic.Death, vfx);
            MusicController.Instance.RemoveAudioSource(_instance);
        }

    }
}