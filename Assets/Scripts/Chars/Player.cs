﻿using System;
using System.Collections.Generic;
using Data;
using DefaultNamespace.Weapons;
using DG.Tweening;
using HexFiled;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chars
{
    struct AnimLength
    {
        public float Move;
        public float Attack;
    }


    public class Player : IUnit
    {
        private HexCoordinates _spawnPos;
        private bool _isAlive;
        private GameObject _instance;
        private GameObject _prefab;
        private AnimLength _animLength;
        private HexCell _cell;
        private HexGrid _hexGrid;
        public Action<GameObject> onPlayerSpawned;
        private Animator _animator;
        private PlayerView _playerView;
        private bool _isBusy;
        private UnitColor _color;
        private float _hp;
        private float _mana;
        private Weapon _weapon;
        private List<HexCell> _cellsToEdge;
        private CharBar _charBar;

        public bool IsBusy => _isBusy;
        public GameObject PlayerInstance => _instance;
        public PlayerView PlayerView => _playerView;

        public Player(PlayerData playerData, Weapon weapon, HexGrid hexGrid)
        {
            _weapon = weapon;
            _spawnPos = playerData.spawnPos;
            _prefab = playerData.playerPrefab;
            _isAlive = false;
            _hexGrid = hexGrid;
            _isBusy = false;
            _color = playerData.color;
            
        }

        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction))
            {
                _isBusy = true;
                _cell = _cell.GetNeighbor(direction);
                _instance.transform.DOLookAt(_cell.transform.position, 0.1f);
                _animator.SetTrigger("Move");
                _animator.SetBool("isMoving", _isBusy);
                _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
            }
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
                _cell = _hexGrid.GetCellFromCoord(_spawnPos);
                _cell.PaintHex(_color);
                for (int i = 0; i < 6; i++)
                {
                    _cell.GetNeighbor((HexDirection)i).PaintHex(_color);
                }

                _instance = Object.Instantiate(_prefab, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
                onPlayerSpawned?.Invoke(_instance);
                _isAlive = true;
                _animator = _instance.GetComponent<Animator>();
                _playerView = _instance.GetComponent<PlayerView>();
                _charBar = _playerView.charBarCanvas.GetComponent<CharBar>();
                SetAnimLength();
                _mana = 100f;
                _hp = 100f;
                SetUpActions();
            }
        }

        private void SetUpActions()
        {
            _playerView.OnStep += () =>
            {
                _isBusy = false;
                _cell.PaintHex(_color);
                _animator.SetBool("isMoving", _isBusy);
            };
            _playerView.OnAttackEnd += () =>
            {
                _isBusy = false;
                _mana -= _weapon.manaCost;
                UpdateCanvas();
                var ball = Object.Instantiate(_weapon.objectToThrow,
                    _instance.transform.position + new Vector3(0, 2), Quaternion.identity);
                var sequence = DOTween.Sequence();
                _cellsToEdge.ForEach(cell =>
                {
                        
                    sequence.Append(ball.transform
                        .DOMove(cell.transform.position + new Vector3(0, 2), _weapon.speed).SetEase(Ease.Linear));
                });
                sequence.onComplete += () => { Object.Destroy(ball); };
                sequence.onUpdate += () =>
                {
                    if (ball == null)
                    {
                        sequence.Kill();
                    }
                };
            };
        }

        private void UpdateCanvas()
        {
            _charBar.ManaBar.fillAmount = _mana / 100;
            _charBar.HealthBar.fillAmount = _hp / 100;
        }

        public void Death()
        {
            throw new NotImplementedException();
        }

        public void Attack(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction) && _mana - _weapon.manaCost >= 0)
            {
                _cellsToEdge = new List<HexCell>();
                _isBusy = true;
                _instance.transform.DOLookAt(_cell.GetNeighbor(direction).transform.position, 0.1f);
                _animator.SetTrigger("Attack");
                var curCell = _cell.GetNeighbor(direction);
                _cellsToEdge.Add(curCell);
                while (curCell.GetNeighbor(direction) != null)
                {
                    curCell = curCell.GetNeighbor(direction);
                    _cellsToEdge.Add(curCell);
                }
            }
        }

        public void Damage(float dmg)
        {
            if (_hp - dmg <= 0f)
            {
                Death();
            }

            _hp -= dmg;
        }
    }
}