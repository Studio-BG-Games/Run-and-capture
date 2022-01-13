using System;
using Data;
using DefaultNamespace;
using HexFiled;
using UnityEngine;

namespace Items
{
    public enum BonusType
    {
        Attack,
        Defence
    }

    [CreateAssetMenu(fileName = "BonusItem", menuName = "Item/Bonus")]
    public class Bonus : Item
    {
        [SerializeField] private float duration;
        [SerializeField] private int value;
        [SerializeField] private BonusType type;
        [SerializeField] private GameObject usisngVFX;

        public BonusType Type => type;
        public void Invoke()
        {
            Unit.SetUpBonus(duration, value, type);
            var vfx = VFXController.Instance.PlayEffect(usisngVFX, Unit.Instance.transform);
            TimerHelper.Instance.StartTimer(() => Destroy(vfx), duration);
            Unit.UseItem(this);
        }
    }
}