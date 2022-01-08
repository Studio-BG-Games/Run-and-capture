using System;
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
        public Action<AIAgent> OnAgentInited;

        public Unit Enemy => _enemy;
        
        public AIAgent(UnitInfo enemyInfo, Unit enemy)
        {
            _enemy = enemy;
            _camera = Camera.main;
            _enemy.OnDeath += AgentDeath;
        }

        private void AgentDeath()
        {
            AIManager.Instance.RemoveAgent(this);
        }
        public void InitAgent(GameObject unit)
        {
            HexManager.agents.Add(unit, this);
            OnAgentInited?.Invoke(this);
        }

        public void Move(Vector2 direction)
        {
            _enemy.Move(DirectionHelper.VectorToDirection(direction));
        }
        public void FixedExecute()
        {
            //throw new System.NotImplementedException();
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