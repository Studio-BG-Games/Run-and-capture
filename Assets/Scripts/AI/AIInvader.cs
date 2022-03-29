using AI;
using HexFiled;
using Units;

namespace DefaultNamespace.AI
{
    public class AIInvader : AIWarior
    {
        public AIInvader(UnitBase unitBase) : base(unitBase)
        {
        }
        public override void FixedExecute()
        {
            if (currentPath.Count > 0 && !_unitBase.IsBusy)
            {
                var dir = currentPath.Dequeue();
                if (!HexManager.UnitCurrentCell.TryGetValue(_unitBase.Color, out var value))
                {
                    return;
                }
                while (value.cell == null)
                {
                    dir = dir.PlusSixtyDeg();
                }
                _unitBase.Move(dir);
            }
            if (currentPath.Count == 0 && !_unitBase.IsBusy)
            {
                curentState = Warior.Instance.GetNewBehaviour(this);
            }
        }
    }
}