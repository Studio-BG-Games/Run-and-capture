using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using HexFiled;
using Runtime.Controller;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Items
{
    public class ItemFabric : IExecute
    {
        private ItemsData _data;
        private List<HexCell> _openList;
        private List<Type> _itemTypes;
        
        private float _spawnTime;
        private float time;

        public ItemFabric(ItemsData data)
        {

            _data = data;
            _openList = new List<HexCell>();
            _spawnTime = Random.Range(data.SpawnTime.from, data.SpawnTime.to);
        }

        public void UpdateCellToOpenList(HexCell cell)
        {
            if (cell.Color != UnitColor.GREY)
            {
                _openList.Add(cell);
            }
            else if (_openList.Contains(cell))
            {
                _openList.Remove(cell);
            }
        }

        public void Execute()
        {
            if (Time.time - time >= _spawnTime)
            {
                List<HexCell> closedList = HexManager.UnitCurrentCell.Select(unitCells => unitCells.Value.cell)
                    .ToList();
                time = Time.time;
                var cell = _openList[Random.Range(0, _openList.Count - 1)];

                if (closedList.Contains(cell) || cell.Item != null)
                {
                    return;
                }

                var i = GetWeightedItemIndex();
                if (i < 0)
                {
                    return;
                }


                _data.ItemInfos[i].Item.Spawn(cell);
                cell.SetItem(_data.ItemInfos[i].Item);
                _spawnTime = Random.Range(_data.SpawnTime.from, _data.SpawnTime.to);
            }
        }

        private int GetWeightedItemIndex()
        {
            float randomNum = Random.Range(1, 101)/100f;
            List<ItemInfos> possibleTypes = new List<ItemInfos>();

            _data.ItemInfos.ForEach(item =>
            {
                if (item.SpawnChance >= randomNum)
                {
                    possibleTypes.Add(item);
                }
            });
            if (possibleTypes.Count > 0)
            {
                return Random.Range(0, possibleTypes.Count - 1);
            }

            return -1;
        }
    }
}