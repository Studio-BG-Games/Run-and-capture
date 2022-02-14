using System;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units;
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

        private GameObject ball;
        public void SetModifiedDamage(int bonus)
        {
            modifiedDamage = damage + bonus;
        }

        public void Fire(Transform start, Vector2 direction, Unit unit)
        {
            ball = Object.Instantiate(objectToThrow,
                start.forward + start.transform.position + new Vector3(0, 2),
                start.rotation);

            MusicController.Instance.AddAudioSource(ball);
            MusicController.Instance.PlayAudioClip(shotSound, ball);
            ball.AddComponent<WeaponView>().SetWeapon(this, unit);
            Weapon tmpThis = this;
            GameObject localBall = ball;
            localBall.transform.DOMove(new Vector3(direction.normalized.x,
                                       0, direction.normalized.y) * tmpThis.disnatce * HexGrid.HexDistance +
                                   start.position + new Vector3(0, 2, 0), tmpThis.speed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    tmpThis.DestroyBall();
                });
        }

        public void DestroyBall()
        {
            if(ball == null)
                return;
            var vfx = VFXController.Instance.PlayEffect(VFXGameObject, ball.transform.position, ball.transform.rotation);
            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayAudioClip(hitSound, vfx);
            ball.transform.DOKill();
            Object.Destroy(ball);
        }
    }
}