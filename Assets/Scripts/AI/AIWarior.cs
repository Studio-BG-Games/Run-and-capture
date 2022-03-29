using AI;
using HexFiled;
using Units;

namespace DefaultNamespace.AI
{
    public class AIWarior : AIBase
    {
        public AIWarior(UnitBase unitBase) : base(unitBase)
        {
        }

        public override void FixedExecute(){
            if (curentState == BotState.Attack && !_unitBase.IsBusy)
            {
                _unitBase.Aim(_attackDirection);
                _unitBase.StartAttack();
                curentState = Unit.Instance.GetNewBehaviour(this);
            }
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

        protected override void InitAgent(UnitBase aiBase)
        {
            AIManager.Instance.AddAgent(this);
            HexManager.agents.Add(aiBase.Instance, this);
            OnAgentInited?.Invoke(this);
        }
    }
}