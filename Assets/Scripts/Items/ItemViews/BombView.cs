using System;
using DefaultNamespace;
using HexFiled;
using Units;
using Units.Views;
using UnityEngine;

namespace Items
{
    public class BombView : MonoBehaviour, ISetUp
    {
        [SerializeField] private int damage;
        [SerializeField] private GameObject hit;
        [SerializeField] private float timeHit;
        private UnitBase _unit;


        public void SetUp(UnitBase unit)
        {
            _unit = unit;
            gameObject.AddComponent<CapsuleCollider>().radius = HexGrid.HexDistance;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var enemy = collision.gameObject.GetComponent<UnitView>();

            if (enemy != null && enemy.Color != _unit.Color)
            {
                
                var vfx = VFXController.Instance.PlayEffect(hit, transform.position, Quaternion.identity);
                vfx.GetComponent<VFXView>().OnTimeInvoke(timeHit, () => enemy.OnHit?.Invoke(damage));
                Destroy(gameObject);
            }
        }
    }
}