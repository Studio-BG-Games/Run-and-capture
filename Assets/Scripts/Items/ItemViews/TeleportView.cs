using System;
using System.Linq;
using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;
using Weapons;

namespace Items.ItemViews
{
    public class TeleportView : MonoBehaviour, ISetUp
    {
        [SerializeField] private GameObject Hit;
        [SerializeField] private GameObject vfx;
        [SerializeField] private int dmg;
        private Unit _unit;
        private GameObject _instance;
   

        public void SetUp(Unit unit)
        {
            _unit = unit;
            _instance = Instantiate(vfx, transform);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var unit = collision.gameObject.GetComponent<UnitView>();
            var weapon = collision.gameObject.GetComponent<WeaponView>();
            if (unit != null && unit.Color != _unit.Color)
            {
                unit.OnHit.Invoke(dmg);
                var cell = HexManager.UnitCurrentCell[unit.Color].cell;

                var dis = HexManager.CellByColor[unit.Color]
                    .Max(x => Vector3.Distance(cell.transform.position, x.transform.position));
                var targetCell = HexManager.CellByColor[unit.Color]
                    .First(x => Math.Abs(Vector3.Distance(cell.transform.position, x.transform.position) - dis) < 0.3f);
                unit.Unit.SetCell(targetCell);
                VFXController.Instance.PlayEffect(Hit, transform).GetComponent<VFXView>().OnPlayEnd += () => Destroy(gameObject);
                Destroy(_instance);
            }

            if (weapon != null && weapon.Unit.Color != _unit.Color)
            {
                Destroy(gameObject);
            }
        }
    }
}
