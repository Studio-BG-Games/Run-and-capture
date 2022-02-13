using System;
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
        private GameObject _aimInstance;
        private HexDirection _direction;
        private Unit chosenUnit;

        public void Invoke(Action<Unit> action)
        {
            OnItemUsed ??= action;

            if (_aimInstance == null)
                _aimInstance = Object.Instantiate(aimCanvas, Unit.Instance.transform);
            _aimInstance.SetActive(false);
        }

        public void Aim(Vector2 direction)
        {
            if (_aimInstance == null)
                _aimInstance = Object.Instantiate(aimCanvas, Unit.Instance.transform);
            _aimInstance.SetActive(true);
            _aimInstance.transform.LookAt(
                new Vector3(direction.x, 0, direction.y) + Unit.Instance.transform.position);

            
            RaycastHit hit;
            Ray ray = new Ray(Unit.Instance.transform.position + new Vector3(0, 1f, 0),
                new Vector3(direction.x, 0, direction.y));
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                Debug.Log($"Aimed {hit.collider.gameObject.name}");
                chosenUnit = hit.collider.gameObject.GetComponent<UnitView>().Unit;
            }

            
            Debug.DrawRay(ray.origin,
                ray.direction * hit.distance, UnityEngine.Color.red);
        }

        public void DeAim()
        {
            _aimInstance.SetActive(false);
        }
    }
}