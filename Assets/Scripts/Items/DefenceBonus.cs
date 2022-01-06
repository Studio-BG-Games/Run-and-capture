using System;
using Data;
using HexFiled;

namespace Items
{
    public class DefenceBonus : Item
    {
        public DefenceBonus(ItemInfo data) : base(data)
        {
        }

        public override void Invoke(Action<Item> item)
        {
            throw new NotImplementedException();
        }

        public override void InstanceInvoke()
        {
            Unit.SetDefenceBonus(Data.Values[0], Data.Values[1]);
            Unit.UseItem(this);
        }

        public override void PlaceItem(HexCell cell)
        {
            throw new NotImplementedException();
        }
    }
}