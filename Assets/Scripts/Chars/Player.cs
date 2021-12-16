using System;
using Data;
using DG.Tweening;
using HexFiled;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chars
{
    public class Player : IUnit
    {
        private HexCoordinates _curentPosition;
        private bool _isAlive;
        private GameObject _instance;
        private GameObject prefab;
        private HexCell _cell;
        private HexGrid _hexGrid;
        private Texture _texture;
        public Action<GameObject> OnPlayerSpawned;
        private Animator _animator;
        private float _tick;

        public GameObject Playerinstance => _instance;

        public Player(PlayerData playerData, HexGrid hexGrid)
        {
            _curentPosition = playerData.spawnPos;
            prefab = playerData.playerPrefab;
            _isAlive = false;
            _hexGrid = hexGrid;
            _texture = playerData.hexTexture;
            _tick = playerData.Tick;
        }


        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction))
            {
                _cell = _cell.GetNeighbor(direction);
                _curentPosition = _cell.coordinates;
                
                _instance.transform.LookAt(_cell.transform);
                _animator.SetTrigger("Move");
                _animator.SetBool("isMoving", true);
                _instance.transform.DOMove(_cell.transform.position, _tick).OnComplete(() =>
                {
                    _animator.SetBool("isMoving", false);
                    _animator.SetTrigger("Jump");
                    _cell.PaintHex(_texture);
                });

            }
        }

        public void Spawn()
        {
            if (!_isAlive)
            {
                _cell = _hexGrid.GetCellFromCoord(_curentPosition);
                _cell.PaintHex(_texture);
                for (int i = 0; i < 6; i++)
                {
                    _cell.GetNeighbor((HexDirection)i).PaintHex(_texture);
                }
                _instance = Object.Instantiate(prefab, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
                OnPlayerSpawned?.Invoke(_instance);
                _isAlive = true;
                _animator = _instance.GetComponent<Animator>();
            }
        }

        public void Death()
        {
            throw new System.NotImplementedException();
        }
    }
}