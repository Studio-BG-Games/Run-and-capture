using Units;
using UnityEngine;

namespace Weapons
{
    public class WeaponView : MonoBehaviour
    {
        public Weapon Weapon { get; private set; }
        public Unit Unit { get; private set; }

        public void SetWeapon(Weapon weapon, Unit unit)
        {
            Weapon = weapon;
            Unit = unit;
        }
    }
}