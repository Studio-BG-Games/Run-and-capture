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

        public Queue<HexDirection> currentPath;
        public Action<AIAgent> OnAgentInited;

        public Unit Enemy => _enemy;

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
        }

        private void InitAgent(GameObject unit)
        {
            _manager.AddAgent(this);
            HexManager.agents.Add(unit, this);
            OnAgentInited?.Invoke(this);
        }
        
        public void FixedExecute()
        {
            if (currentPath.Count > 0 && !_enemy.IsBusy)
            {
                _enemy.Move(currentPath.Dequeue());
            }
            else if(currentPath.Count == 0)
            {
                _manager.SetBehaviour(BotState.Patrol, this);
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