using Data;
using HexFiled;
using UnityEngine;

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

        public Player(PlayerData playerData, HexGrid hexGrid)
        {
            _curentPosition = playerData.SpawnPos;
            prefab = playerData.PlayerPrefab;
            _isAlive = false;
            _hexGrid = hexGrid;
            
        }

        public void Move(HexCoordinates coordinates)
        {
            throw new System.NotImplementedException();
        }

        public void Spawn()
        {
            if (!_isAlive)
            {
                _cell = _hexGrid.GetCellFromCoord(_curentPosition);
                _instance = Object.Instantiate(prefab, _cell.transform.position, Quaternion.identity);
            }
        }

        public void Death()
        {
            throw new System.NotImplementedException();
        }
    }
}