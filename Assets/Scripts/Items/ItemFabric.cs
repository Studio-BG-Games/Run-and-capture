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
                List<HexCell> closedList = PaintedController.UnitCurrentCell.Select(unitCells => unitCells.Value.curent)
                    .ToList();
                time = Time.time;
                var cell = _openList[Random.Range(0, _openList.Count - 1)];
                
                while (closedList.Contains(cell) || cell.Item != null)
                {
                    cell = _openList[Random.Range(0, _openList.Count - 1)];
                }
                
                var type = _itemTypes[Random.Range(0, _itemTypes.Count - 1)];
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
    }
}