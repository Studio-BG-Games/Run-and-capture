using Data;
using DG.Tweening;
using HexFiled;
using Weapons;

namespace Units.Wariors.AbstractsBase
{
    public abstract class Invader : Warior
    {
        protected Invader(WariorInfo data, Weapon weapon, HexGrid hexGrid, UnitColor spawnerColor) : base(data, weapon, hexGrid, spawnerColor)
        {
        }
                public override void Move(HexDirection direction)
        {
            base.Move(direction);
            if (_cell.GetNeighbor(direction).Color == Color ||
                (_cell.GetNeighbor(direction).Color == _easyCaptureColor && _easyCaptureColor != UnitColor.Grey))
            {
                DoTransit(direction);
            }
            else if (_cell.GetNeighbor(direction).Color != UnitColor.Grey)
            {
                if (_mana - _hexGrid.HexHardCaptureCost <= 0) return;
                IsHardToCapture = true;
                DoTransit(direction);
            }

            else if (_mana - _hexGrid.HexCaptureCost >= 0)
            {
                if (_mana - _hexGrid.HexHardCaptureCost <= 0) return;
                DoTransit(direction);
            }
        }
        public override void StartAttack()
        {
        }
                public override void SetCell(HexCell cell, bool isInstanceTrans = false, bool isPaintingHex = false)
        {
            _cell = cell;
            HexManager.UnitCurrentCell[Color] = (cell, this);
            if (!isInstanceTrans)
            {
                IsBusy = true;
                _instance.transform.DOMove(_cell.transform.position, _animLength.SuperJump)
                    .OnComplete(() => IsBusy = false);
            }
            else
            {
                _instance.transform.DOMove(_cell.transform.position, 0.5f).SetEase(Ease.Linear);
            }

            if (isPaintingHex)
            {
                cell.PaintHex(Color, true);
            }
        }

        protected override void CaptureHex()
        {
            if (!_isInfiniteMana)
            {
                if (IsHardToCapture)
                {
                    _mana -= _hexGrid.HexHardCaptureCost;
                }
                else
                {
                    _mana -= _hexGrid.HexCaptureCost;
                }
            }

            BaseView.RegenMana();


            UpdateBarCanvas();
            IsBusy = false;
            IsHardToCapture = false;
            _cell.PaintHex(Color);
        }
    }
}