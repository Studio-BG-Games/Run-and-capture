using System.Collections;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units.Wariors.AbstractsBase;
using UnityEngine;
using Weapons;

namespace Units.Views
{
    public class WariorView : ViewBase
    {
        protected override void OnTriggerEnter(Collider other)
        {
            var weaponView = other.GetComponent<WeaponView>();
            if (weaponView != null && weaponView.Unit.Color != _unit.Color)
            {
                OnHit?.Invoke(weaponView.Weapon.modifiedDamage);

                var vfx = VFXController.Instance.PlayEffect(weaponView.Weapon.VFXGameObject,
                    transform.position + new Vector3(0, 2, 0),
                    weaponView.Weapon.VFXGameObject.transform.rotation);
                MusicController.Instance.AddAudioSource(vfx);
                MusicController.Instance.PlayAudioClip(weaponView.Weapon.hitSound, vfx);

                other.transform.DOKill();
                Destroy(other.gameObject);
            }
        }

        protected override IEnumerator Regen()
        {
            if (_mana >= ((Warior)_unit).Data.maxMana)
            {
                yield break;
            }

            while (_mana < ((Warior)_unit).Data.maxMana)
            {
                yield return new WaitForSeconds(1f);
                _mana += _manaRegen;
                _startRegen.Invoke();
            }
            
        }
    }
}