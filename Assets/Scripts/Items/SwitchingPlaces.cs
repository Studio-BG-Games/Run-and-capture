using System;
using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Items
{
    [CreateAssetMenu(fileName = "SwitchingPlaces", menuName = "Item/Switching Places")]
    public class SwitchingPlaces : Item
    {
        [SerializeField] private GameObject aimCanvas;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float time;
        private GameObject _aimInstance;
        private HexDirection _direction;


        public void Invoke(Action<Unit> action, Unit unit)
        {
            if (!unit.IsPlayer)
            {
                return;
            }

            OnItemUsed ??= action;

            if (_aimInstance == null)
                _aimInstance = Object.Instantiate(aimCanvas, unit.Instance.transform);
            _aimInstance.SetActive(false);
        }

        public void Aim(Vector2 direction, Unit unit, out Unit chosenUnit)
        {
            if (unit.IsPlayer)
            {
                if (_aimInstance == null)
                    _aimInstance = Object.Instantiate(aimCanvas, unit.Instance.transform);
                _aimInstance.SetActive(true);
                _aimInstance.transform.LookAt(
                    new Vector3(direction.x, 0, direction.y) + unit.Instance.transform.position);
            }

            RaycastHit hit;
            Ray ray = new Ray(unit.Instance.transform.position + new Vector3(0, 1f, 0),
                new Vector3(direction.x, 0, direction.y));

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                Debug.Log($"Aimed {hit.collider.gameObject.name}");
                chosenUnit = hit.collider.gameObject.GetComponent<UnitView>().Unit;
            }

            else
            {
                chosenUnit = null;
            }

            Debug.DrawRay(ray.origin,
                ray.direction * hit.distance, UnityEngine.Color.red);
        }


        public void UseAbility(Unit unit, Unit chosenUnit)
        {
            unit.UseItem(this);
            DeAim();
            OnItemUsed?.Invoke(unit);
            chosenUnit.IsBusy = true;
            chosenUnit.IsStaned = true;
            var unitCell = HexManager.UnitCurrentCell[unit.Color].cell;
            var choseUnitCell = HexManager.UnitCurrentCell[chosenUnit.Color].cell;
            unit.SetCell(choseUnitCell, true, true);
            unit.SetEasyColor(chosenUnit.Color, time);
            chosenUnit.SetCell(unitCell, true);
            TimerHelper.Instance.StartTimer(() =>
            {
                chosenUnit.SetCell(choseUnitCell, true, true);
                unit.SetCell(unitCell, true);
                chosenUnit.IsStaned = false;
                chosenUnit.IsBusy = false;
            }, time);
        }

        public void DeAim()
        {
            _aimInstance.SetActive(false);
        }
    }
}