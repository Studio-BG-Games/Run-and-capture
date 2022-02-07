using System;
using DefaultNamespace;
using HexFiled;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;

namespace Items
{
    [CreateAssetMenu(fileName = "SpecialWeapon", menuName = "Item/Special Weapon")]
    public class SpecialWeapon : Item
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private GameObject _aimGameObject;

        private GameObject _aimInstance;
        private HexDirection _direction;

        public void Invoke(Action action)
        {
            OnItemUsed ??= action;
            
            if(_aimInstance == null || !_aimInstance.activeSelf)
                _aimInstance = Object.Instantiate(_aimGameObject, Unit.Instance.transform);
            _aimInstance.SetActive(false);
        }
        
        public void Aim(HexDirection direction)
        {
            _aimInstance.SetActive(true);
            _aimInstance.transform.LookAt(HexManager.UnitCurrentCell[Unit.Color].cell
                .GetNeighbor(direction).transform);
            _direction = direction;
        }
        
        public void DeAim()
        {
            _aimInstance.SetActive(false);
        }

        public void Fire()
        {
            var cell = HexManager.UnitCurrentCell[Unit.Color].cell.GetNeighbor(_direction);
            Unit.RotateUnit(new Vector2((cell.transform.position - Unit.Instance.transform.position).normalized.x,
                (cell.transform.position - Unit.Instance.transform.position).normalized.z));
            _aimInstance.SetActive(false);
            var dir = DirectionHelper.DirectionTo(Unit.Instance.transform.position, cell.transform.position);
            _weapon.Fire(Unit.Instance.transform, new Vector2(dir.x, dir.z));
            OnItemUsed?.Invoke();
        }
    }
}