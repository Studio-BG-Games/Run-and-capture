using System;
using System.ComponentModel;
using DefaultNamespace;
using HexFiled;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "BuildingItem", menuName = "Item/Building")]
    public class Building : Item
    {

        [SerializeField] private GameObject buildingPrefab;
        
        
        public void Invoke(Action action)
        {
            OnItemUsed += action;
        }

        
        public void PlaceItem(HexCell cell)
        {
            Unit.UseItem(this);
            var obj = SpawnHelper.Spawn(buildingPrefab, cell.transform.position + buildingPrefab.transform.position);
            obj.GetComponent<TowerView>()?.SetUp(Unit.Color);
            obj.GetComponent<BombView>()?.SetUp(Unit);
            cell.Building =  obj;
            OnItemUsed.Invoke();
            OnItemUsed = null;
        }
    }
}