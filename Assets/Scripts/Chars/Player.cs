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
        private HexCoordinates spawnPos;
        private bool _isAlive;
        private GameObject _instance;
        private GameObject prefab;
        private AnimLength _animLength;
        private HexCell _cell;
        private HexGrid _hexGrid;
        private Texture _texture;
        public Action<GameObject> OnPlayerSpawned;
        private Animator _animator;
        private PlayerView _playerView;
        private bool _isMoving;
        private GameObject _vfxPrefab;
        private UnitColor _color;
        private static readonly int Moving = Animator.StringToHash("isMoving");
        private static readonly int Move1 = Animator.StringToHash("Move");

        public bool IsMoving => _isMoving;
        public GameObject PlayerInstance => _instance;

        public Player(PlayerData playerData, HexGrid hexGrid)
        {
            spawnPos = playerData.spawnPos;
            prefab = playerData.playerPrefab;
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
                _animLength.Move = clip.name switch
                {
                    "Jump" => clip.length,
                    _ => _animLength.Move
                };
            }
        }

        public void Spawn()
        {
            if (!_isAlive)
            {
                _cell = _hexGrid.GetCellFromCoord(spawnPos);
                _cell.PaintHex(_color);
                for (int i = 0; i < 6; i++)
                {
                    _cell.GetNeighbor((HexDirection)i).PaintHex(_color);
                }

                _instance = Object.Instantiate(prefab, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
                OnPlayerSpawned?.Invoke(_instance);
                _isAlive = true;
                _animator = _instance.GetComponent<Animator>();
                _playerView = _instance.GetComponent<PlayerView>();
                SetAnimLength();
            }
        }

        public void Death()
        {
            throw new System.NotImplementedException();
        }
    }
}