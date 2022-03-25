using System;
using Data;
using HexFiled;
using Weapons;

namespace Units.Wariors.AbstractsBase
{
    public abstract class Patrol : Warior
    {
        protected Patrol(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor) : base(data, weapon, hexGrid, spawnerColor)
        {
        }

        public override void SetCell(HexCell cell, bool isInstanceTrans = false, bool isPaintingHex = false)
        {
        }

        protected override void CaptureHex()
        {
        }
        public override void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction) == null || _cell.GetNeighbor(direction).BuildingInstance != null ||
        IsBusy || IsHardToCapture ||
        (_cell.GetNeighbor(direction).Color != Color
         && HexManager.UnitCurrentCell.TryGetValue(_cell.GetNeighbor(direction).Color, out var value)
         && value.cell.Equals(_cell.GetNeighbor(direction)))) return;

            if (_cell.GetNeighbor(direction).Color != Color) return;
            DoTransit(direction);
        }
    }
}