using System;
using System.Collections.Generic;
using AI;
using Controller;
using HexFiled;
using Units;
using UnityEngine;

namespace DefaultNamespace.AI
{
    public abstract class AIBase : IFixedExecute, IDisposable
    {
        protected UnitBase _unitBase;
        private Camera _camera;
        protected BotState curentState;
        public Queue<HexDirection> currentPath;
        public Action<AIBase> OnAgentInited;
        protected Vector2 _attackDirection;
        
        public UnitBase UnitBase => _unitBase;

        public BotState CurentState => curentState;
        
        public AIBase(UnitBase unitBase)
        {
            currentPath = new Queue<HexDirection>();
            _camera = Camera.main;
            _unitBase = unitBase;
            _unitBase.OnDeath += AgentDeath;
            _unitBase.OnSpawned += InitAgent;
        }

        
        protected virtual void AgentDeath(UnitBase aiBase)
        {
            AIManager.Instance.RemoveAgent(this);
            currentPath.Clear();
        }

        protected abstract void InitAgent(UnitBase aiBase);

        public void AttackTarget(Vector2 direction)
        {
            _attackDirection = direction;
            
        }
        
        public abstract void FixedExecute();


        public void Dispose()
        {
        }
    }
}