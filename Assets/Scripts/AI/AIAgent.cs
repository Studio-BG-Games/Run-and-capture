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
    public class AIAgent : IFixedExecute, IExecute
    {
        private Unit _enemy;
        private Camera _camera;
        private AIManager _manager;
        private BotState curentState;
        public Queue<HexDirection> currentPath;
        public Action<AIAgent> OnAgentInited;
        private Vector2 _attackDirection;

        public Unit Enemy => _enemy;

        public BotState CurentState => curentState;

        public AIAgent(UnitInfo enemyInfo, Unit enemy, AIManager manager)
        {
            currentPath = new Queue<HexDirection>();
            _enemy = enemy;
            _camera = Camera.main;
            _enemy.OnDeath += AgentDeath;
            enemy.onPlayerSpawned += InitAgent;
            _manager = manager;
        }

        
        private void AgentDeath(Unit unit)
        {
            AIManager.Instance.RemoveAgent(this);
            currentPath.Clear();
        }

        private void InitAgent(GameObject unit)
        {
            _manager.AddAgent(this);
            HexManager.agents.Add(unit, this);
            OnAgentInited?.Invoke(this);
        }

        public void AttackTarget(Vector2 direction)
        {
            _attackDirection = direction;
            
        }
        
        public void FixedExecute()
        {
            if (curentState == BotState.Attack && !_enemy.IsBusy)
            {
                _enemy.Aim(_attackDirection);
                _enemy.StartAttack();
                curentState = _manager.GetNewBehaviour(this);
            }
            if (currentPath.Count > 0 && !_enemy.IsBusy)
            {
                var dir = currentPath.Dequeue();
                while (HexManager.UnitCurrentCell[_enemy.Color].cell.GetNeighbor(dir) == null)
                {
                    dir = dir.PlusSixtyDeg();
                }
                _enemy.Move(dir);
            }
            if(currentPath.Count == 0 && !_enemy.IsBusy)
            {
                curentState = _manager.GetNewBehaviour(this);
            }
        }

        public void Execute()
        {
            if (_enemy.UnitView != null)
            {
                _enemy.UnitView.BarCanvas.transform.DOLookAt(
                    _enemy.UnitView.BarCanvas.transform.position + _camera.transform.rotation * Vector3.back, 0f,
                    up: _camera.transform.rotation * Vector3.up);
            }
        }
    }
}