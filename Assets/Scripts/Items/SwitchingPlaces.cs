using System;
using DefaultNamespace;
using HexFiled;
using Units;
using Units.Views;
using UnityEngine;
using Object = UnityEngine.Object;
using Unit = Units.Unit.Unit;

namespace Items
{
    [CreateAssetMenu(fileName = "SwitchingPlaces", menuName = "Item/Switching Places")]
    public class SwitchingPlaces : Item
    {
        [SerializeField] private GameObject aimCanvas;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float time;


        public override void Invoke(ItemContainer container)
        {
            var unit = (Unit) container.Unit;
            if (!unit.IsPlayer)
            {
                return;
            }


            if (container.AimInstance == null)
                container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
            container.AimInstance.SetActive(false);
        }

        public void Aim(Vector2 direction, ItemContainer container)
        {
            var unit = (Unit) container.Unit;
            if (unit.IsPlayer)
            {
                if (container.AimInstance == null)
                    container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
                container.AimInstance.SetActive(true);
                container.AimInstance.transform.LookAt(
                    new Vector3(direction.x, 0, direction.y) + container.Unit.Instance.transform.position);
            }

            RaycastHit hit;
            Ray ray = new Ray(container.Unit.Instance.transform.position + new Vector3(0, 1f, 0),
                new Vector3(direction.x, 0, direction.y));

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                Debug.Log($"Aimed {hit.collider.gameObject.name}");
                container.Value = hit.collider.gameObject.GetComponent<UnitView>().Unit;
            }


            Debug.DrawRay(ray.origin,
                ray.direction * hit.distance, UnityEngine.Color.red, 10f);
        }


        public void UseAbility(ItemContainer container)
        {
            if (container.Value == null || container.Unit.isSwitched)
            {
                container.DeAim();
                return;
            }

            container.Unit.UseItem(this);
            container.Unit.isSwitched = true;
            container.DeAim();
            container.OnItemUsed?.Invoke();
            container.Value.IsBusy = true;
            container.Value.IsStaned = true;
            var unitCell = HexManager.UnitCurrentCell[container.Unit.Color].cell;
            var choseUnitCell = HexManager.UnitCurrentCell[container.Value.Color].cell;
            container.Unit.SetCell(choseUnitCell, true, true);
            container.Unit.SetEasyColor(container.Value.Color, time);
            container.Value.SetCell(unitCell, true);
            TimerHelper.Instance.StartTimer(() =>
            {
                container.Unit.isSwitched = false;
                container.Value.SetCell(choseUnitCell, true, true);
                container.Unit.SetCell(unitCell, true);
                container.Value.IsStaned = false;
                container.Value.IsBusy = false;
            }, time);
        }
    }
}