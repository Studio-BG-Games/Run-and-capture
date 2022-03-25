using Data;
using HexFiled;
using Weapons;

namespace Units.Wariors.AbstractsBase
{
    public class TestInvader : Invader
    {
        public TestInvader(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor) : base(data, weapon, hexGrid, spawnerColor)
        {
        }
    }
}