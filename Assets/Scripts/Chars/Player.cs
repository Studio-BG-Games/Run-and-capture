using System;
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
        private Vector3 _direction;
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

        private void Step()
        {
            _isBusy = false;
            _cell.PaintHex(_color);
            _animator.SetBool("isMoving", _isBusy);
        }

        private void AttackEnd()
        {
            _isBusy = false;
            _mana -= _weapon.manaCost;
            UpdateCanvas();
        }

        private void Attacking()
        {
            var ball = Object.Instantiate(_weapon.objectToThrow,
                _instance.transform.position + new Vector3(0, 2), Quaternion.identity);
            ball.transform.DOMove(new Vector3(_direction.x * 100,2, _direction.y * 100), _weapon.speed)
                .SetEase(Ease.Linear)
                .OnComplete(() => Object.Destroy(ball));
        }

        private void SetUpActions()
        {
            _playerView.OnStep += Step;
            _playerView.OnAttackEnd += AttackEnd;
            _playerView.OnAttack += Attacking;
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


        public void Attack(Vector2 direction)
        {
            _isBusy = true;
            _animator.SetTrigger("Attack");
        }

        public void Aim(Vector2 direction)
        {
            _playerView.transform.LookAt(new Vector3(direction.x,0, direction.y) + _playerView.transform.position);
            _direction = direction;
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