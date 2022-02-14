using System;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units;
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
        [SerializeField] private float lifeTime;

        private GameObject _aimInstance;
        private HexDirection _direction;

        public void Invoke(Action<Unit> action, Unit unit)
        {
            OnItemUsed ??= action;
            
            if(_aimInstance == null || !_aimInstance.activeSelf)
                _aimInstance = Object.Instantiate(_aimGameObject, unit.Instance.transform);
            _aimInstance.SetActive(false);
        }
        
        public void Aim(HexDirection direction, Unit unit)
        {
            _aimInstance.SetActive(true);
            _aimInstance.transform.LookAt(HexManager.UnitCurrentCell[unit.Color].cell
                .GetNeighbor(direction).transform);
            _direction = direction;
        }
        
        public void DeAim()
        {
            _aimInstance.SetActive(false);
        }

        public void Fire(Unit unit)
        {
            OnItemUsed?.Invoke(unit);
            unit.UseItem(this);
            var cell = HexManager.UnitCurrentCell[unit.Color].cell.GetNeighbor(_direction);
            unit.RotateUnit(new Vector2((cell.transform.position - unit.Instance.transform.position).normalized.x,
                (cell.transform.position - unit.Instance.transform.position).normalized.z));
            _weapon.SetModifiedDamage(0);
            _weapon.objectToThrow.GetComponent<ISetUp>().SetUp(unit);
            _aimInstance.SetActive(false);
            var dir = DirectionHelper.DirectionTo(unit.Instance.transform.position, cell.transform.position);
            _weapon.Fire(unit.Instance.transform, new Vector2(dir.x, dir.z), unit);
            TimerHelper.Instance.StartTimer(() =>
            {
                _weapon.DestroyBall();
            }, lifeTime);
            
        }
    }
}