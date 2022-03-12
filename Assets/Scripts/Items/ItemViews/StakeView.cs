using System.Threading.Tasks;
using DefaultNamespace;
using Units;
using Units.Views;
using UnityEngine;
using Weapons;

namespace Items.ItemViews
{
    public class StakeView : MonoBehaviour, ISetUp
    {
        [SerializeField] private int instanceDamage;
        [SerializeField] private int timingDamage;
        [SerializeField] private int time;
        [SerializeField] private GameObject catchVfx;
        [SerializeField] private GameObject destroed;
        private UnitBase _unit;

        public void SetUp(UnitBase unit)
        {
            _unit = unit;
        }

        private void OnCollisionEnter(Collision collisionInfo)
        {
            var unit = collisionInfo.gameObject.GetComponent<UnitView>();
            var weapon = collisionInfo.gameObject.GetComponent<WeaponView>();
            if (unit != null && unit.Color != _unit.Color)
            {
                unit.OnHit.Invoke(instanceDamage);
                StartDamage(unit);
                VFXController.Instance.PlayEffect(catchVfx, transform);
            }

            if (weapon != null && weapon.Unit.Color != _unit.Color)
            {
                VFXController.Instance.PlayEffect(destroed, transform.position);
                Destroy(gameObject);
            }
        }

        private async void StartDamage(UnitView unit)
        {
            for (int i = 0; i < time; i++)
            {
                await DoTimingDamage(unit);
            }
        }

        private async Task DoTimingDamage(UnitView unit)
        {
            await Task.Delay(1000);
            unit.OnHit.Invoke(timingDamage);
        }
    }
}