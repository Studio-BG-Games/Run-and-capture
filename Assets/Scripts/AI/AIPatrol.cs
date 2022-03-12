using AI;
using HexFiled;
using Units;

namespace DefaultNamespace.AI
{
    public class AIPatrol : AIBase
    {
        public AIPatrol(UnitBase unitBase) : base(unitBase)
        {
        }

        protected override void InitAgent(UnitBase aiBase)
        {
            AIManager.Instance.AddAgent(this);
            HexManager.agents.Add(aiBase.Instance, this);
            OnAgentInited?.Invoke(this);
        }
    }
}