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
        private Action _action;

        public void Invoke(Action action)
        {
            if(_action != null) return;
            _action = action;
            OnItemUsed += _action;
        }


        public void PlaceItem(HexCell cell)
        {
            Unit.UseItem(this);
            var obj = Instantiate(buildingPrefab,
                cell.transform.position + buildingPrefab.transform.position, Quaternion.identity);
            obj.GetComponent<ISetUp>().SetUp(Unit);
            
            cell.Building = obj;
            OnItemUsed.Invoke();
            OnItemUsed = _action;
        }
    }
}