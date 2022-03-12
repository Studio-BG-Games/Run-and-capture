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
        private BotState curentState;
        public Queue<HexDirection> currentPath;
        public Action<AIBase> OnAgentInited;
        private Vector2 _attackDirection;
        
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
        
        public virtual void FixedExecute()
        {
            if (curentState == BotState.Attack && !_unitBase.IsBusy)
            {
                _unitBase.Aim(_attackDirection);
                _unitBase.StartAttack();
                curentState = AIManager.Instance.GetNewBehaviour(this);
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
            if(currentPath.Count == 0 && !_unitBase.IsBusy)
            {
                curentState = AIManager.Instance.GetNewBehaviour(this);
            }
        }


        public void Dispose()
        {
        }
    }
}