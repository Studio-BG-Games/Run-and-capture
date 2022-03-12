using System.Collections.Generic;
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
        public Holem(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor) : base(data, weapon,
            hexGrid, spawnerColor)
        {
        }

        protected override void DoTransit(HexDirection direction)
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

        public override void Spawn(HexCoordinates hexCoordinates, HexCell spawnCell = null)
        {
            if (!IsAlive)
            {
                _cell = spawnCell != null ? spawnCell : _hexGrid.GetCellFromCoord(hexCoordinates);
                IsVisible = true;
                _cell.PaintHex(_data.color, true);
                _cell.GetListNeighbours().ForEach(x => { x?.PaintHex(Color, true); });
                _inventory = new List<ItemContainer>();
                _inventoryDefence = new List<ItemContainer>();

                _instance = Object.Instantiate(_data.wariorPrefa, _cell.transform.parent);

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
                OnOnPlayerSpawned(this);
            }
        }

        public override void Death()
        {
            UnitView.OnStep -= MoveEnd;
            UnitView.OnAttackEnd -= AttackEnd;
            UnitView.OnAttack -= Attacking;
            UnitView.OnHit -= Damage;
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

        protected override void Attacking()
        {
            Aim(_direction);

            _weapon.Fire(_instance.transform, _direction, this);
        }
    }
}