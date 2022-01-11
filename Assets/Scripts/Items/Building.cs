using System;
using Data;
using DefaultNamespace;
using HexFiled;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Items
{
    [CreateAssetMenu(fileName = "BuildingItem", menuName = "Item/Building")]
    public class Building : Item
    {

        [SerializeField] private GameObject buildingPrefab;
        
        public void Invoke(Action<Item> action)
        {
            Unit.UseItem(this);
            OnItemUsed += action;
        }

        
        public void PlaceItem(HexCell cell)
        {
            Unit.UseItem(this);
            var obj = SpawnHelper.Spawn(buildingPrefab, cell.transform.position + buildingPrefab.transform.position);
            obj.GetComponent<TowerView>().SetUp(Unit.Color);
            cell.Building =  obj.GetComponent<TowerView>();
            OnItemUsed?.Invoke(this);
        }
    }
}