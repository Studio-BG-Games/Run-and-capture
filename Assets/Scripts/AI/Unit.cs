using System;
using System.Linq;
using Data;
using DefaultNamespace.AI;
using Items;
using UnityEngine;

namespace AI
{
    public sealed class Unit : AIManager
    {
        public Unit(AIData data) : base(data)
        {
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
                case BotState.CollectingBonus:
                    StartPatrolBehaviour(agent);
                    break;
                case BotState.ProtectBonusUsage:
                    StartPatrolBehaviour(agent);
                    break;
                case BotState.AttackBonusUsage:
                    StartPatrolBehaviour(agent);
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

        public override BotState GetNewBehaviour(AIBase agent)
        {
            var attack = agent.UnitBase.Inventory.Where(x => x.Item is Bonus { BonusType: BonusType.Attack }).ToList();
            if (agent.CurentState is BotState.Attack && agent.UnitBase.AttackBonus == 0 && attack.Count > 0)
            {
                SetBehaviour(BotState.AttackBonusUsage, agent, 500);
                return BotState.AttackBonusUsage;
            }


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

            var item = GetNearestItem(agent);
            if (item.hex != null)
            {
                if ((item.dist <= _data.DistaceToCollectBonus ||
                     agent.UnitBase.Mana <= agent.UnitBase.maxMana * _data.ManaPercentToCollectBonus) &&
                    (item.hex.Item.Item.Type == ItemType.DEFENCE
                        ? agent.UnitBase.InventoryDefence.Count
                        : agent.UnitBase.Inventory.Count) < agent.UnitBase.InventoryCapacity / 2)
                {
                    SetBehaviour(BotState.CollectingBonus, agent, 500);
                    return BotState.CollectingBonus;
                }
            }


            var protect = agent.UnitBase.InventoryDefence.Where(x => x.Item is Bonus { BonusType: BonusType.Defence })
                .ToList();
            if (protect.Count > 0 && agent.UnitBase.Hp <= agent.UnitBase.maxHP * _data.PercentToUseProtectBonus &&
                agent.UnitBase.DefenceBonus == 0)
            {
                SetBehaviour(BotState.ProtectBonusUsage, agent, 500);
                return BotState.ProtectBonusUsage;
            }


            SetBehaviour(BotState.Patrol, agent, 500);
            return BotState.Patrol;
        }
    }
}