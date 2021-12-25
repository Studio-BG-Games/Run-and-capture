using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DefaultNamespace.Weapons;
using DG.Tweening;
using HexFiled;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Chars
{
    public class Player : IUnit
    {
        private bool _isAlive;
        private GameObject _instance;

        private AnimLength _animLength;
        private HexCell _cell;
        private HexGrid _hexGrid;
        public Action<GameObject> onPlayerSpawned;
        private Animator _animator;
        private UnitView _unitView;
        private bool _isBusy;
        private PlayerData _data;
        private int _hp;
        private int _mana;
        private Weapon _weapon;
        private Vector2 _direction;
        private BarCanvas _barCanvas;


        public bool IsBusy => _isBusy;
        public GameObject PlayerInstance => _instance;
        public UnitView UnitView => _unitView;

        public Player(PlayerData playerData, Weapon weapon, HexGrid hexGrid)
        {
            _weapon = weapon;
            _data = playerData;
            _isAlive = false;
            _hexGrid = hexGrid;
            _isBusy = false;
        }

        public void Move(HexDirection direction)
        {
            if (!_cell.GetNeighbor(direction) || _isBusy) return;
            if (_cell.GetNeighbor(direction).Color == _data.color)
            {
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
            _cell = _cell.GetNeighbor(direction);
            _instance.transform.DOLookAt(_cell.transform.position, 0.1f);
            _animator.SetTrigger("Move");
            _animator.SetBool("isMoving", _isBusy);
            _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
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

        public void Spawn()
        {
            if (!_isAlive)
            {
                _cell = _hexGrid.GetCellFromCoord(_data.spawnPos);
                _cell.PaintHex(_data.color);
                for (int i = 0; i < 6; i++)
                {
                    _cell.GetNeighbor((HexDirection)i).PaintHex(_data.color);
                }

                _instance = Object.Instantiate(_data.playerPrefab, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
                onPlayerSpawned?.Invoke(_instance);
                _isAlive = true;
                _animator = _instance.GetComponent<Animator>();
                _unitView = _instance.GetComponent<UnitView>();
                _barCanvas = _unitView.BarCanvas.GetComponent<BarCanvas>();
                _unitView.SetUp(_barCanvas.SpawnShotUI(_weapon.shots), _weapon, RegenMana, _data.manaRegen);
                SetAnimLength();
                _mana = _data.maxMana;
                _hp = _data.maxHP;
                SetUpActions();
            }
        }

        private void RegenMana()
        {
            _mana += _data.manaRegen;
            UpdateBarCanvas();
        }

        private void MoveEnd()
        {
            _isBusy = false;
            _cell.PaintHex(_data.color);
            _animator.SetBool("isMoving", _isBusy);
        }

        private void AttackEnd()
        {
            _isBusy = false;
            UpdateBarCanvas();
        }

        private void Attacking()
        {
            var ball = Object.Instantiate(_weapon.objectToThrow,
                _instance.transform.forward + _instance.transform.position + new Vector3(0, 2), _instance.transform.rotation);
            ball.GetComponent<WeaponView>().SetWeapon(_weapon);
            ball.transform.DOMove(
                    new Vector3(_direction.normalized.x,
                        0, _direction.normalized.y) * _weapon.disnatce * _hexGrid.HexDistance +
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
            _barCanvas.HealthBar.DOFillAmount(hp / maxHp, 0.5f).SetEase(Ease.InQuad);
        }

        public void Death()
        {
            throw new NotImplementedException();
        }


        public void StartAttack(Vector2 direction)
        {
            if (!_isBusy && _unitView.Shoot())
            {
                _isBusy = true;
                _animator.SetTrigger("Attack");
            }
        }

        public void Aim(Vector2 direction)
        {
            _unitView.transform.LookAt(new Vector3(direction.x, 0, direction.y) + _unitView.transform.position);
            _direction = direction;
        }

        public void Damage(int dmg)
        {
            if (_hp - dmg <= 0f)
            {
                Death();
            }

            _hp -= dmg;
        }
    }
}