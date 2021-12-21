using System;
using Data;
using DG.Tweening;
using HexFiled;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chars
{
    struct AnimLength
    {
        public float Move;
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
        private bool _isMoving;
        private UnitColor _color;
        private static readonly int Moving = Animator.StringToHash("isMoving");
        private static readonly int Move1 = Animator.StringToHash("Move");
        private float _hp;
        private float _mana;

        public bool IsMoving => _isMoving;
        public GameObject PlayerInstance => _instance;
        public PlayerView PlayerView => _playerView;

        public Player(PlayerData playerData, HexGrid hexGrid)
        {
            _spawnPos = playerData.spawnPos;
            _prefab = playerData.playerPrefab;
            _isAlive = false;
            _hexGrid = hexGrid;
            _isMoving = false;
            _color = playerData.color;
        }

        

        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction))
            {
                _isMoving = true;
                _cell = _cell.GetNeighbor(direction);
                _instance.transform.LookAt(_cell.transform);
                _animator.SetTrigger(Move1);
                _animator.SetBool(Moving, _isMoving);
                _playerView.OnStep += () =>
                {
                    _isMoving = false;
                    _cell.PaintHex(_color);

                    _animator.SetBool(Moving, _isMoving);
                };
                
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
                    case "Jump":
                        _animLength.Move = clip.length;
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
                SetAnimLength();
            }
        }

        public void Death()
        {
            throw new NotImplementedException();
        }

        public void Attack(HexDirection direction)
        {
            throw new NotImplementedException();
        }

        public void Damag(float dmg)
        {
            throw new NotImplementedException();
        }
    }
}