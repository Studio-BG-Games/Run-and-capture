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
        private Dictionary<string, ItemInfo> _itemInfos;
        private float _spawnTime;
        private float time;

        public ItemFabric(ItemsData data, List<Type> dictionary)
        {
            _itemInfos = new Dictionary<string, ItemInfo>();
            data.ItemInfos.ForEach(info => { _itemInfos.Add(info.Type, info); });
            _itemTypes = dictionary;
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

                var type = GetWeightedType();
                if (type == null)
                {
                    return;
                }
                var info = _itemInfos[type.ToString().Replace("Items.", "")];
                var obj = (Item)Activator.CreateInstance(type, info);

                var go = obj.Spawn(cell);
                go.AddComponent<CapsuleCollider>().isTrigger = true;
                var itemView = go.AddComponent<ItemView>();
                itemView.SetUp(obj);
                cell.SetItem(obj);
                _spawnTime = Random.Range(_data.SpawnTime.from, _data.SpawnTime.to);
            }
        }

        private Type GetWeightedType()
        {
            float randomNum = Random.Range(1, 101)/100f;
            List<Type> possibleTypes = new List<Type>();

            _itemTypes.ForEach(type =>
            {
                if (_itemInfos[type.ToString().Replace("Items.", "")].SpawnChance >= randomNum)
                {
                    possibleTypes.Add(type);
                }
            });
            return possibleTypes.Count > 0 ? possibleTypes[Random.Range(0, possibleTypes.Count - 1)] : null;
        }
    }
}