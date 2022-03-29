using System;
using System.Linq;
using Data;
using DefaultNamespace.AI;
using UnityEngine;

namespace AI
{
    public class Invader : Warior
    {
        public Invader(AIData data) : base(data)
        {
        }
        public override BotState GetNewBehaviour(AIBase agent)
        {
            var enemy = GetNearestUnit(_data.DistanceToAgr, agent.UnitBase);
                if (enemy != null && agent.UnitBase.Hp > agent.UnitBase.maxHP * _data.PercentToRetreet && enemy.IsAlive)
                {
                    if (agent.UnitBase.Hp <= agent.UnitBase.maxHP * _data.PercentToRetreet ||
                        agent.UnitBase.BaseView.AvailableShots == 0)
                    {
                        SetBehaviour(BotState.Retreet, agent, 500);
                        return BotState.Retreet;
                    }
                }

                SetBehaviour(BotState.Patrol, agent, 500);
                return BotState.Patrol;
        }
    }
    public class Warior : AIManager
    {
        public Warior(AIData data) : base(data)
        {
        }

        public override BotState GetNewBehaviour(AIBase agent)
        {
                var enemy = GetNearestUnit(_data.DistanceToAgr, agent.UnitBase);
                if (enemy != null && agent.UnitBase.Hp > agent.UnitBase.maxHP * _data.PercentToRetreet && enemy.IsAlive)
                {
                    if (agent.UnitBase.Hp <= agent.UnitBase.maxHP * _data.PercentToRetreet ||
                        agent.UnitBase.BaseView.AvailableShots == 0)
                    {
                        SetBehaviour(BotState.Retreet, agent, 500);
                        return BotState.Retreet;
                    }

                    if (Vector3.Distance(agent.UnitBase.Instance.transform.position, enemy.Instance.transform.position) <=
                        agent.UnitBase.Weapon.disnatce)
                    {
                        SetBehaviour(BotState.Attack, agent, 500);
                        return BotState.Attack;
                    }

                    SetBehaviour(BotState.Agressive, agent, 500);
                    return BotState.Agressive;
                }

                SetBehaviour(BotState.Patrol, agent, 500);
                return BotState.Patrol;
            }

        protected override void SetBehaviour(BotState state, AIBase agent, int dist)
        {
            switch (state)
            {
                case BotState.Patrol:
                    StartPatrolBehaviour(agent);
                    break;
                case BotState.Agressive:
                    MoveToEnemy(agent, dist);
                    break;
                case BotState.Attack:
                    AttackEnemy(agent);
                    break;
                case BotState.Dead:
                    break;
                case BotState.Retreet:
                    Retreet(agent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected override void InitAI(AIBase agent)
        {
            SetBehaviour(BotState.Patrol, agent, 500);
        }
    }
    }