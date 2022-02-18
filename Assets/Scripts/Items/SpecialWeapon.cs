using System;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
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
        [SerializeField] private float lifeTime;
        


        public override void Invoke(ItemContainer container)
        {
            
            if(container.AimInstance == null)
                container.AimInstance = Object.Instantiate(_aimGameObject, container.Unit.Instance.transform);
            container.AimInstance.SetActive(false);
        }

       
        public void Aim(ItemContainer container, HexDirection direction)
        {
            container.AimInstance.SetActive(true);
            container.AimInstance.transform.LookAt(HexManager.UnitCurrentCell[container.Unit.Color].cell
                .GetNeighbor(container.Direction).transform);
            container.Direction = direction;
        }
        

        public void Fire(ItemContainer container)
        {
            container.OnItemUsed.Invoke();
            container.Unit.UseItem(this);
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell.GetNeighbor(container.Direction);
            container.Unit.RotateUnit(new Vector2((cell.transform.position - container.Unit.Instance.transform.position).normalized.x,
                (cell.transform.position - container.Unit.Instance.transform.position).normalized.z));
            _weapon.SetModifiedDamage(0);
            _weapon.objectToThrow.GetComponent<ISetUp>().SetUp(container.Unit);
            container.DeAim();
            var dir = DirectionHelper.DirectionTo(container.Unit.Instance.transform.position, cell.transform.position);
            var ball = _weapon.Fire(container.Unit.Instance.transform, new Vector2(dir.x, dir.z), container.Unit);
            TimerHelper.Instance.StartTimer(() =>
            {
                if(ball == null)
                    return;
                var vfx = VFXController.Instance.PlayEffect(_weapon.VFXGameObject, ball.transform.position, Quaternion.identity);
                MusicController.Instance.AddAudioSource(vfx);
                MusicController.Instance.PlayAudioClip(_weapon.hitSound, vfx);
                ball.transform.DOKill();
                Object.Destroy(ball);
            }, lifeTime);
            
        }
    }
}