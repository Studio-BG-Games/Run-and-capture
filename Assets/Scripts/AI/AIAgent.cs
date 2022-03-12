using System;
using System.Collections.Generic;
using AI;
using Controller;
using Data;
using DG.Tweening;
using HexFiled;
using Runtime.Controller;
using Units;
using Units.Wariors.AbstractsBase;
using UnityEngine;

namespace DefaultNamespace.AI
{
    public class AIAgent : AIBase
    {
        public AIAgent(UnitBase unitBase) : base(unitBase)
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