using Data;
using DG.Tweening;
using HexFiled;
using Weapons;

namespace Units.Wariors.AbstractsBase
{
    public class AssailantTest : Assailant
    {
        public AssailantTest(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor) : base(data, weapon, hexGrid, spawnerColor)
        {
        }
    }
}