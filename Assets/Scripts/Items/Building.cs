using System;
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
            Unit.UseItem(this);
            OnItemUsed += action;
        }

        
        public void PlaceItem(HexCell cell)
        {
            Unit.UseItem(this);
            var obj = SpawnHelper.Spawn(buildingPrefab, cell.transform.position + buildingPrefab.transform.position);
            obj.GetComponent<TowerView>().SetUp(Unit.Color);
            cell.Building =  obj.GetComponent<TowerView>();
            OnItemUsed?.Invoke();
        }
    }
}