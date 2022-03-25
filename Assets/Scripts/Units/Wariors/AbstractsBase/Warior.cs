using System.Linq;
using AI;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units.Views;
using Items;
using UnityEngine;
using Weapons;
using System.Collections.Generic;

namespace Units.Wariors.AbstractsBase
{
    public abstract class Warior : UnitBase
    {
        protected WariorInfo _data;
        public WariorInfo Data => _data;


        public Warior(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor)
        {
            Initialize(weapon, hexGrid);
            _data = data;
            Color = spawnerColor;
            maxHP = _data.maxHP;
            maxMana = _data.maxMana;
            hexGrid.OnHexPainted += Death;
        }

        public override void Spawn(HexCoordinates hexCoordinates, HexCell spawnCell = null)
        {
            if (!IsAlive)
            {
                _cell = spawnCell != null ? spawnCell : _hexGrid.GetCellFromCoord(hexCoordinates);
                IsVisible = true;
                _cell.PaintHex(Color, true);
                _cell.GetListNeighbours().ForEach(x => { x?.PaintHex(Color, true); });
                _inventory = new List<ItemContainer>();
                _inventoryDefence = new List<ItemContainer>();

                _instance = UnityEngine.Object.Instantiate(_data.wariorPrefab, _cell.transform.parent);

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
        public override void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction) == null || _cell.GetNeighbor(direction).BuildingInstance != null ||
    IsBusy || IsHardToCapture ||
    (_cell.GetNeighbor(direction).Color != Color
     && HexManager.UnitCurrentCell.TryGetValue(_cell.GetNeighbor(direction).Color, out var value)
     && value.cell.Equals(_cell.GetNeighbor(direction)))) return;
        }
        protected override void DoTransit(HexDirection direction)
        {
            IsBusy = true;
            _isCapturing = Color != _cell.GetNeighbor(direction).Color;
            _cell = _cell.GetNeighbor(direction);
            RotateUnit(new Vector2((_cell.transform.position - _instance.transform.position).normalized.x,
                (_cell.transform.position - _instance.transform.position).normalized.z));
            _animator.SetTrigger("Move");
            _animator.SetBool("isMoving", IsBusy);
            _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
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



        protected override void UpdateBarCanvas()
        {
            if (_hp > maxHP)
                _hp = maxHP;
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
                UnityEngine.Object.Destroy(_instance);
                OnOnDeath(this);
            }, _animLength.Death);

            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(MusicController.Instance.MusicData.SfxMusic.Death, vfx);
            MusicController.Instance.RemoveAudioSource(_instance);
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
        private void Death(HexCell cell)
        {
            OnOnDeath(this);
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