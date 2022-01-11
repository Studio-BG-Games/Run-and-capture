using System;
using Data;
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
        public void Invoke()
        {
            Unit.SetUpBonus(duration, value, type);
            Unit.UseItem(this);
            
        }

       
    }
}