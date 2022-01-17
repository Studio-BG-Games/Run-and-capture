﻿using System;
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
        public static Dictionary<GameObject, HexCell> Items;
        private ItemsData _data;
        private List<HexCell> _openList;
        private List<Type> _itemTypes;
        private GameObject _itemParrant;
        private float _spawnTime;
        private float time;

        public ItemFabric(ItemsData data)
        {
            _itemParrant = new GameObject("Items");
            Items = new Dictionary<GameObject, HexCell>();
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
                if (_openList.Count == 0)
                {
                    return;
                }
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


                Items.Add(_data.ItemInfos[i].Item.Spawn(cell, _itemParrant), cell);
                cell.SetItem(_data.ItemInfos[i].Item);
                _spawnTime = Random.Range(_data.SpawnTime.from, _data.SpawnTime.to);
            }
        }

        private int GetWeightedItemIndex()
        {
            float randomNum = Random.Range(1, 101)/100f;
            int[] possibleTypes = new int[_data.ItemInfos.Count];
            var i = 0;
            var j = 0;
            _data.ItemInfos.ForEach(item =>
            {
                
                if (item.SpawnChance >= randomNum)
                {
                    possibleTypes[j++] = i;
                }

                ++i;
            });
            if (j > 0)
            {
                return possibleTypes[Random.Range(0, j - 1)];
            }

            return -1;
        }
    }
}