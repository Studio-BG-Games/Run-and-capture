using System;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Weapons
{
    [Serializable]
    public struct Weapon
    {
        public string name;
        public GameObject icon;
        public GameObject objectToThrow;
        public GameObject VFXGameObject;
        public int modifiedDamage;
        public int damage;
        public float speed;
        public int disnatce;
        public float reloadTime;
        public int shots;
        public AudioClip shotSound;
        public AudioClip hitSound;

        public void SetModifiedDamage(int bonus)
        {
            modifiedDamage = damage + bonus;
        }

        public void Fire(Transform start, Vector2 direction)
        {
            var ball = Object.Instantiate(objectToThrow,
                start.forward + start.transform.position + new Vector3(0, 2),
                start.rotation);

            MusicController.Instance.AddAudioSource(ball);
            MusicController.Instance.PlayAudioClip(shotSound, ball);
            ball.AddComponent<WeaponView>().SetWeapon(this);
            Weapon tmpThis = this;
            ball.transform.DOMove(new Vector3(direction.normalized.x,
                                      0, direction.normalized.y) * tmpThis.disnatce * HexGrid.HexDistance +
                                  start.position + new Vector3(0, 2, 0), tmpThis.speed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    var vfx = VFXController.Instance.PlayEffect(tmpThis.VFXGameObject, ball.transform);
                    MusicController.Instance.AddAudioSource(vfx);
                    MusicController.Instance.PlayAudioClip(tmpThis.hitSound, vfx);
                    Object.Destroy(ball);
                });
        }
    }
}