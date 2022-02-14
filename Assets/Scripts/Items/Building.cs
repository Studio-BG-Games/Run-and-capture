using System;
using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "BuildingItem", menuName = "Item/Building")]
    public class Building : Item
    {
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private bool isVisiting = false;
        

        public void Invoke(Action<Unit> action)
        {
            OnItemUsed += action;
        }


        public void PlaceItem(HexCell cell, Unit unit)
        {
            unit.UseItem(this);
            var obj = Instantiate(buildingPrefab,
                cell.transform.position + buildingPrefab.transform.position, Quaternion.identity);
            obj.GetComponent<ISetUp>().SetUp(unit);
            if (!isVisiting)
            {
                cell.Building = buildingPrefab;
                cell.BuildingInstance = obj;
            }

            OnItemUsed.Invoke(unit);
            OnItemUsed = null;
        }
    }
}