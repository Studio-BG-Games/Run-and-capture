using Units;
using UnityEngine;

namespace Weapons
{
    public class WeaponView : MonoBehaviour
    {
        public Weapon Weapon { get; private set; }
        public UnitBase Unit { get; private set; }

        public void SetWeapon(Weapon weapon, UnitBase unit)
        {
            Weapon = weapon;
            Unit = unit;
        }
    }
}