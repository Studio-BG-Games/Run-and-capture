using UnityEngine;

namespace Weapons
{
    public class WeaponView : MonoBehaviour
    {
        public Weapon Weapon { get; private set; }

        public void SetWeapon(Weapon weapon)
        {
            Weapon = weapon;
        }

        
   
    }
}
