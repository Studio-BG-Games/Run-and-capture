using System;
using Data;
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

        public GameObject Playerinstance => _instance;

        public Player(PlayerData playerData, HexGrid hexGrid)
        {
            _curentPosition = playerData.spawnPos;
            prefab = playerData.playerPrefab;
            _isAlive = false;
            _hexGrid = hexGrid;
            _texture = playerData.hexTexture;
        }


        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction))
            {
                _cell = _cell.GetNeighbor(direction);
                _curentPosition = _cell.coordinates;
                _cell.PaintHex(_texture);
                _instance.transform.localPosition = _cell.transform.localPosition;
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
            }
        }

        public void Death()
        {
            throw new System.NotImplementedException();
        }
    }
}