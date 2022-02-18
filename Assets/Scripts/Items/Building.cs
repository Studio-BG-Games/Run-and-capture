using System;
using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Items
{
    [CreateAssetMenu(fileName = "BuildingItem", menuName = "Item/Building")]
    public class Building : Item
    {
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private bool isVisiting = false;
        [SerializeField] private GameObject aimCanvas;
        

        public override void Invoke(ItemContainer container)
        {

            if (container.AimInstance == null)
            {
                container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
            }
            container.AimInstance.SetActive(false);
        }
        
        public void Aim(HexDirection direction, ItemContainer container)
        {
            if (container.AimInstance == null)
            {
                container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
            }
            container.AimInstance.SetActive(true);
            
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell
                .GetNeighbor(direction);
            if (cell == null)
            {
                return;
            }
            container.AimInstance.transform.LookAt(cell.transform);
            container.Direction = direction;
        }


        public void PlaceItem(ItemContainer container)
        {
            container.Unit.UseItem(this);
            container.DeAim();
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell.GetNeighbor(container.Direction);
            var obj = Instantiate(buildingPrefab,
                cell.transform.position + buildingPrefab.transform.position, Quaternion.identity);
            obj.GetComponent<ISetUp>().SetUp(container.Unit);
            if (!isVisiting)
            {
                cell.Building = buildingPrefab;
                cell.BuildingInstance = obj;
            }

            container.OnItemUsed.Invoke();
            
        }
    }
}