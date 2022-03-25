using AI;
using Data;
using HexFiled;
using Units.Wariors.AbstractsBase;
using Weapons;

namespace Units.Wariors
{
    public class Holem : Patrol
    {
        public Holem(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor) : base(data, weapon, hexGrid, spawnerColor)
        {
        }
    }
}