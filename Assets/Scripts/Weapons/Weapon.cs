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

      
        public void SetModifiedDamage(int bonus)
        {
            modifiedDamage = damage + bonus;
        }

        public GameObject Fire(Transform start, Vector2 direction, Unit unit, bool isMoving = true)
        {
            var ball = Object.Instantiate(objectToThrow,
                start.forward + start.transform.position + new Vector3(0, 1),
                start.rotation);

            MusicController.Instance.AddAudioSource(ball);
            MusicController.Instance.PlayAudioClip(shotSound, ball);
            ball.AddComponent<WeaponView>().SetWeapon(this, unit);
            Weapon tmpThis = this;
            
            Weapon tmpThis1 = this;
            if (isMoving)
            {
                ball.transform.DOMove(new Vector3(direction.normalized.x,
                                          0, direction.normalized.y) * tmpThis.disnatce * HexGrid.HexDistance +
                                      start.position + new Vector3(0, 1, 0), tmpThis.speed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if(ball == null)
                            return;
                        var vfx = VFXController.Instance.PlayEffect(tmpThis1.VFXGameObject, ball.transform.position, Quaternion.identity);
                        if (vfx != null)
                        {
                            MusicController.Instance.AddAudioSource(vfx);
                            MusicController.Instance.PlayAudioClip(tmpThis1.hitSound, vfx);
                        }

                        ball.transform.DOKill();
                        Object.Destroy(ball);
                    });
            }
           
            return ball;
        }

    
    }
}