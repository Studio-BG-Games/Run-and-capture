using System;
using System.Collections.Generic;
using Chars;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Items;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


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
        private int _attackBonus;
        private int _defenceBonus;


        public bool IsBusy { get =>  _isBusy; set => _isBusy = value; }
        public UnitView UnitView => _unitView;
        public bool IsAlive => _isAlive;
        public UnitColor Color => _data.color;
        public int InventoryCapacity => _data.inventoryCapacity;
        public Action<Item> OnItemPickUp;
        public Action<Unit> OnDeath; 
        public BarCanvas BarCanvas => _barCanvas;

        public UnitInfo Data => _data;

        public Unit(UnitInfo unitData, Weapon weapon, HexGrid hexGrid)
        {
            _weapon = weapon;
            _data = unitData;
            _isAlive = false;
            _hexGrid = hexGrid;
            _isBusy = false;
            _isHardToCapture = false;
            _isCapturing = false;
        }

        public void SetAttackBonus(int duration, int value)
        {
            TimerHelper.Instance.StartTimer(StopAttackBonus, duration);
            _weapon.SetModifiedDamage(value);
        }

        private void StopAttackBonus()
        {
            _weapon.SetModifiedDamage(0);
        }

        public void SetDefenceBonus(int duration, int value)
        {
            TimerHelper.Instance.StartTimer(StopDefenceBonus, duration);
            _defenceBonus = value;
        }

        private void StopDefenceBonus()
        {
            _defenceBonus = 0;
        }

        public void Move(HexDirection direction)
        {
            if (!_cell.GetNeighbor(direction) || _isBusy || _cell.GetNeighbor(direction).Color != UnitColor.GREY &&
                HexManager.UnitCurrentCell[_cell.GetNeighbor(direction).Color].cell == _cell.GetNeighbor(direction)) return;
            _unitView.StopHardCapture();
            if (_cell.GetNeighbor(direction).Color == _data.color)
            {
                DoTransit(direction);
            }
            else if (_cell.GetNeighbor(direction).Color != UnitColor.GREY)
            {
                _isHardToCapture = true;
                _unitView.RegenMana(_mana);
                DoTransit(direction);
            }

            else if (_mana - _hexGrid.HexCaptureCost >= 0)
            {
                _mana -= _hexGrid.HexCaptureCost;
                _unitView.RegenMana(_mana);
                UpdateBarCanvas();
                DoTransit(direction);
            }
        }

        private void DoTransit(HexDirection direction)
        {
            _isBusy = true;
            _isCapturing = _data.color != _cell.GetNeighbor(direction).Color;
            var previousCell = _cell;
            _cell = _cell.GetNeighbor(direction);
            HexManager.UnitCurrentCell[_data.color] = ( _cell, this );
            RotateUnit(new Vector2((_cell.transform.position - _instance.transform.position).normalized.x,
                (_cell.transform.position - _instance.transform.position).normalized.z));
            _animator.SetTrigger("Move");
            _animator.SetBool("isMoving", _isBusy);
            _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
        }

        private void CaptureHex()
        {
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
                _inventory = new List<Item>();
                for (int i = 0; i < 6; i++)
                {
                    var neigh = _cell.GetNeighbor((HexDirection)i);
                    neigh?.PaintHex(_data.color);

                    for (int j = 0; j < 6; j++)
                    {
                        neigh?.GetNeighbor((HexDirection)j)?.PaintHex(_data.color);
                    }
                }

                //
                HexManager.UnitCurrentCell.Add(_data.color, (_cell, this));
                //

                _instance = Object.Instantiate(_data.unitPrefa, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
               
                _isAlive = true;
                _animator = _instance.GetComponent<Animator>();
                _unitView = _instance.GetComponent<UnitView>();
                _barCanvas = _unitView.BarCanvas.GetComponent<BarCanvas>();
                _unitView.SetUp(_barCanvas.SpawnShotUI(_weapon.shots), _weapon, RegenMana, _data.manaRegen, CaptureHex,
                    this);
                SetAnimLength();
                MusicController.Instance.AddAudioSource(_instance);
                _mana = _data.maxMana;
                _hp = _data.maxHP;
                SetUpActions();
                onPlayerSpawned?.Invoke(_instance);
            }
        }

        private void RegenMana()
        {
            _mana += _data.manaRegen;
            UpdateBarCanvas();
        }

        public bool PickUpItem(ItemView itemView)
        {
            if (_inventory.Count < _data.inventoryCapacity)
            {
                var item = itemView.PickUp(this);
                _inventory.Add(item);
                OnItemPickUp.Invoke(item);
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
                _isHardToCapture = false;
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
            if (_direction.Equals(Vector2.zero))
            {
                _direction = new Vector2(_unitView.transform.forward.x, _unitView.transform.forward.z);
                Aim(_direction);
            }

            var ball = Object.Instantiate(_weapon.objectToThrow,
                _instance.transform.forward + _instance.transform.position + new Vector3(0, 2),
                _instance.transform.rotation);
            MusicController.Instance.AddAudioSource(ball);
            MusicController.Instance.PlayAudioClip(_weapon.shotSound, ball);
            ball.AddComponent<WeaponView>().SetWeapon(_weapon);
            ball.transform.DOMove(
                    new Vector3(_direction.normalized.x,
                        0, _direction.normalized.y) * _weapon.disnatce * HexGrid.HexDistance +
                    _instance.transform.position + new Vector3(0, 2, 0),
                    _weapon.speed)
                .SetEase(Ease.Linear)
                .OnComplete(() => Object.Destroy(ball));
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
            //_barCanvas.ManaBar.value = 
            //_unitView.RegenMana(10);
            _barCanvas.HealthBar.DOFillAmount(hp / maxHp, 0.5f).SetEase(Ease.InQuad);
        }

        public void Death()
        {
            _unitView.OnStep -= MoveEnd;
            _unitView.OnAttackEnd -= AttackEnd;
            _unitView.OnAttack -= Attacking;
            _unitView.OnHit -= Damage;
            _isAlive = false;
            HexManager.UnitCurrentCell.Remove(Color);
            _animator.SetTrigger("Death");
            OnDeath?.Invoke(this);
            MusicController.Instance.PlayAudioClip(MusicController.Instance.MusicData.SfxMusic.Death, _instance);
            MusicController.Instance.RemoveAudioSource(_instance);
            HexManager.PaintHexList(HexManager.CellByColor[Color], UnitColor.GREY);
            Object.Destroy(_instance);
        }


        public void StartAttack()
        {
            if (!_isBusy && _unitView.Shoot())
            {
                _isBusy = true;
                if (!_direction.Equals(Vector2.zero))
                    RotateUnit(_direction);

                _animator.SetTrigger("Attack");
            }
        }

        private void RotateUnit(Vector2 direction)
        {
            _unitView.transform.DOLookAt(new Vector3(direction.x, 0, direction.y) + _unitView.transform.position,
                0.1f);
        }

        public void Aim(Vector2 direction)
        {
            _unitView.AimCanvas.transform.LookAt(
                new Vector3(direction.x, 0, direction.y) + _unitView.transform.position);
            _direction = direction;
        }

        public HexCell PlaceItemAim(HexDirection direction)
        {
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
                _hp -= dmg;
            }

            UpdateBarCanvas();
        }
    }
}