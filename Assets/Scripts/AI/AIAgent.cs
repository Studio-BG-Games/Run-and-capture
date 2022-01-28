using System;
using System.Collections.Generic;
using AI;
using Controller;
using Data;
using DG.Tweening;
using HexFiled;
using Runtime.Controller;
using Units;
using UnityEngine;

namespace DefaultNamespace.AI
{
    public class AIAgent : IFixedExecute, IDisposable
    {
        private Unit _unit;
        private Camera _camera;
        private BotState curentState;
        public Queue<HexDirection> currentPath;
        public Action<AIAgent> OnAgentInited;
        private Vector2 _attackDirection;

        public Unit Unit => _unit;

        public BotState CurentState => curentState;

        public AIAgent(UnitInfo enemyInfo, Unit unit)
        {
            currentPath = new Queue<HexDirection>();
            _unit = unit;
            _camera = Camera.main;
            _unit.OnDeath += AgentDeath;
            unit.OnPlayerSpawned += InitAgent;
            
        }

        
        private void AgentDeath(Unit unit)
        {
            AIManager.Instance.RemoveAgent(this);
            currentPath.Clear();
        }

        private void InitAgent(Unit unit)
        {
            AIManager.Instance.AddAgent(this);
            HexManager.agents.Add(unit.Instance, this);
            OnAgentInited?.Invoke(this);
        }

        public void AttackTarget(Vector2 direction)
        {
            _attackDirection = direction;
            
        }
        
        public void FixedExecute()
        {
            if (curentState == BotState.Attack && !_unit.IsBusy)
            {
                _unit.Aim(_attackDirection);
                _unit.StartAttack();
                curentState = AIManager.Instance.GetNewBehaviour(this);
            }
            if (currentPath.Count > 0 && !_unit.IsBusy)
            {
                var dir = currentPath.Dequeue();
                if (!HexManager.UnitCurrentCell.TryGetValue(_unit.Color, out var value))
                {
                    return;
                }
                while (value.cell == null)
                {
                    dir = dir.PlusSixtyDeg();
                }
                _unit.Move(dir);
            }
            if(currentPath.Count == 0 && !_unit.IsBusy)
            {
                curentState = AIManager.Instance.GetNewBehaviour(this);
            }
        }


        public void Dispose()
        {
        }
    }
}