using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using DefaultNamespace;
using DefaultNamespace.AI;
using HexFiled;
using Runtime.Controller;
using UnityEngine;

namespace AI
{
    public class AIManager : IFixedExecute
    {
        private List<AIAgent> _agents;
        private int _triesToCalculatePath = 0;
        private int _maxTriesToCalculatePath = 5;
        private Dictionary<AIAgent, Queue<HexCell>> _pathToPatrol;
        private static AIManager _instance;

        public static AIManager Instance
        {
            get => _instance;
            private set => _instance ??= value;
        }

        public AIManager(List<AIAgent> agents)
        {
            _agents = agents;
            _pathToPatrol = new Dictionary<AIAgent, Queue<HexCell>>();
            agents.ForEach(agent =>
            {
                _pathToPatrol.Add(agent, new Queue<HexCell>());
                SetBehaviour(BotState.Patrol, agent);
            });
        }

        public AIManager()
        {
            _agents = new List<AIAgent>();
            _pathToPatrol = new Dictionary<AIAgent, Queue<HexCell>>();
            Instance = this;
            HexManager.agents = new Dictionary<GameObject, AIAgent>();
        }

        public void AddAgent(AIAgent agent)
        {
            _agents.Add(agent);

            agent.OnAgentInited += InitAI;
            
        }

        public void RemoveAgent(AIAgent agent)
        {
            _agents.Remove(agent);
            agent.OnAgentInited -= InitAI;
        }  
        
        private void InitAI(AIAgent agent)
        {
            _pathToPatrol.Add(agent, new Queue<HexCell>());
            SetBehaviour(BotState.Patrol, agent);
        }

        private void StartPatrolBehaviour(AIAgent agent)
        {
            HexManager.GetNearestDifferCell(agent.Enemy.Color, _pathToPatrol[agent]);
            while (_pathToPatrol[agent] == null && _triesToCalculatePath < _maxTriesToCalculatePath)
            {
                HexManager.GetNearestDifferCell(agent.Enemy.Color, _pathToPatrol[agent]);
                _triesToCalculatePath++;
            }

            MoveAlongPath(agent);
        }


        private void SetBehaviour(BotState state, AIAgent agent)
        {
            switch (state)
            {
                case BotState.Patrol:
                    MoveAlongPath(agent);
                    break;
                case BotState.Agressive:
                    // MoveToEnemy(_currentEnemy);
                    break;
                case BotState.Attack:
                    // AttackEnemy(_currentEnemy);
                    break;
                case BotState.CollectingBonus:
                    // MoveToBonus(_currentTargetTile);
                    break;
                case BotState.ProtectBonusUsage:
                    // PlaceProtectBonus(_playerState.currentTile);
                    break;
                case BotState.AttackBonusUsage:
                    break;
                case BotState.Dead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void MoveAlongPath(AIAgent agent)
        {
            //Debug.Log("try to move next point");
            if (_pathToPatrol != null && _pathToPatrol[agent].Count > 0) //recalculate existing path or start anew one
            {
                var start = HexManager.UnitCurrentCell[agent.Enemy.Color].cell.transform.position;
                var end = _pathToPatrol[agent].Dequeue().transform.position;
                var dir = DirectionHelper.DirectionTo(start, end);
                agent.Move(new Vector2(dir.x, dir.z));
            }
            else
            {
                StartPatrolBehaviour(agent);
            }
        }


        public void FixedExecute()
        {
            for (int i = 0; i < _pathToPatrol.Count; i++)
            {
                if (!_agents[i].Enemy.IsBusy)
                    SetBehaviour(BotState.Patrol, _agents[i]);
            }
        }
    }


    public enum BotState
    {
        Patrol,
        Agressive,
        Attack,
        CollectingBonus,
        AttackBonusUsage,
        ProtectBonusUsage,
        Dead
    }
}