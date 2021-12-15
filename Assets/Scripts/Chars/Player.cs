using System;
using Data;
using HexFiled;
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
        public Action<GameObject> OnPlayerSpawned;

        public GameObject Playerinstance => _instance;

        public Player(PlayerData playerData, HexGrid hexGrid)
        {
            _curentPosition = playerData.spawnPos;
            prefab = playerData.playerPrefab;
            _isAlive = false;
            _hexGrid = hexGrid;
        }


        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction))
            {
                _cell = _cell.GetNeighbor(direction);
                _curentPosition = _cell.coordinates;
                
                _instance.transform.localPosition = _cell.transform.localPosition;
            }
        }

        public void Spawn()
        {
            if (!_isAlive)
            {
                _cell = _hexGrid.GetCellFromCoord(_curentPosition);
                _instance = Object.Instantiate(prefab, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
                OnPlayerSpawned?.Invoke(_instance);
            }
        }

        public void Death()
        {
            throw new System.NotImplementedException();
        }
    }
}