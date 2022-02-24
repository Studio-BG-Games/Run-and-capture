using System;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Sirenix.OdinInspector;
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
        [SerializeField] private bool isLifeByTime = true;
        [SerializeField, ShowIf("isLifeByTime")] private float lifeTime;
        


        public override void Invoke(ItemContainer container)
        {
            
            if(container.AimInstance == null)
                container.AimInstance = Object.Instantiate(_aimGameObject, container.Unit.Instance.transform);
            container.AimInstance.SetActive(false);
        }

       
        public void Aim(ItemContainer container, Vector2 direction)
        {
           
            container.AimInstance.SetActive(true);
            container.AimInstance.transform.LookAt(
                    new Vector3(direction.x, 0, direction.y) + container.Unit.UnitView.transform.position);
            container.Direction = direction;
        }
        

        public void Fire(ItemContainer container)
        {
            container.OnItemUsed.Invoke();
            container.Unit.UseItem(this);
           
            container.Unit.RotateUnit(container.Direction);
            _weapon.SetModifiedDamage(0);
            container.DeAim();
            
            TimerHelper.Instance.StartTimer(() =>
            {
                var ball = _weapon.Fire(container.Unit.Instance.transform, container.Direction, container.Unit, false);
                ball.GetComponent<ISetUp>().SetUp(container.Unit);
                if (isLifeByTime)
                {
                    TimerHelper.Instance.StartTimer(() =>
                    {
                        if (ball == null)
                            return;
                        var vfx = VFXController.Instance.PlayEffect(_weapon.VFXGameObject, ball.transform.position,
                            Quaternion.identity);
                        MusicController.Instance.AddAudioSource(vfx);
                        MusicController.Instance.PlayAudioClip(_weapon.hitSound, vfx);
                        ball.transform.DOKill();
                        Object.Destroy(ball);
                    }, lifeTime);
                }
            }, 0.1f);
            
        }
    }
}