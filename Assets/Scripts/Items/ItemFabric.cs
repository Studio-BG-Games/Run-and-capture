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
        public static Dictionary<GameObject, HexCell> Items;
        public static Dictionary<ItemType, GameObject> itemIcon;
        private ItemsData _data;
        private List<Type> _itemTypes;
        private GameObject _itemParrant;
        private float _spawnTime;
        private float time;

        public ItemFabric(ItemsData data)
        {
            itemIcon = new Dictionary<ItemType, GameObject>();
            _itemParrant = new GameObject("Items");
            Items = new Dictionary<GameObject, HexCell>();
            _data = data;
            _spawnTime = Random.Range(data.SpawnTime.from, data.SpawnTime.to);
            data.Icons.ForEach(icon =>
            {
                itemIcon.Add(icon.Type, icon.Prefab);
            });
        }

       

        public void Execute()
        {
            if (Time.time - time >= _spawnTime)
            {
                List<HexCell> closedList = HexManager.UnitCurrentCell
                    .Select(unitCells => unitCells.Value.cell)
                    .ToList(); 
                
                foreach (var cellByColor 
                         in HexManager.CellByColor
                             .Where(cellByColor => cellByColor.Key != UnitColor.Grey))
                {
                    cellByColor.Value.ForEach(x =>
                    {
                        if (x.Building != null && x.Item != null)
                        {
                            closedList.Add(x);
                        }
                    });
                }

                List<HexCell> openList = new List<HexCell>();
                time = Time.time;
                foreach (var cellByColor 
                         in HexManager.CellByColor
                             .Where(cellByColor => cellByColor.Key != UnitColor.Grey))
                {
                    openList.AddRange(cellByColor.Value);
                }
                
                    
                if (HexManager.CellByColor.Count == 0)
                {
                    return;
                }
                var cell = openList[Random.Range(0, openList.Count - 1)];

                if (closedList.Contains(cell) || cell.Item != null)
                {
                    return;
                }

                var i = GetWeightedItemIndex();
                if (i < 0)
                {
                    return;
                }

                var item = _data.ItemInfos[i].Item;
                Items.Add(item.Spawn(cell, _itemParrant, itemIcon[item.Type]), cell);
                
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