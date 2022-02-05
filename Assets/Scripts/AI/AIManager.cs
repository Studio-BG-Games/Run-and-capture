using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using DefaultNamespace;
using DefaultNamespace.AI;
using HexFiled;
using Items;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class AIManager
    {
        private int _triesToCalculatePath = 0;
        private int _maxTriesToCalculatePath = 5;
        private AIData _data;

        private static AIManager _instance;


        public static AIManager Instance
        {
            get => _instance;
            private set => _instance ??= value;
        }


        public AIManager(AIData data)
        {
            _data = data;
            Instance = this;
            HexManager.agents = new Dictionary<GameObject, AIAgent>();
        }

        public void AddAgent(AIAgent agent)
        {
            agent.OnAgentInited += InitAI;
        }

        public void RemoveAgent(AIAgent agent)
        {
            agent.OnAgentInited -= InitAI;
        }

        private void InitAI(AIAgent agent)
        {
            SetBehaviour(BotState.Patrol, agent);
        }

        private void StartPatrolBehaviour(AIAgent agent)
        {
            HexManager.GetNearestDifferCell(agent.Unit.Color, agent.currentPath);
            while (agent.currentPath.Count == 0 && _triesToCalculatePath < _maxTriesToCalculatePath)
            {
                HexManager.GetNearestDifferCell(agent.Unit.Color, agent.currentPath);
                _triesToCalculatePath++;
            }

            _triesToCalculatePath = 0;
        }

        public static Unit GetNearestUnit(int cellDist, Unit agent)
        {
            List<(float dist, Unit unit)> res = new List<(float, Unit)>();
            try
            {
                foreach (var color in (UnitColor[])Enum.GetValues(typeof(UnitColor)))
                {
                    if (HexManager.UnitCurrentCell.ContainsKey(color) &&
                        HexManager.UnitCurrentCell[color] != (null, null) &&
                        Vector3.Distance(HexManager.UnitCurrentCell[color].unit.Instance.transform.position,
                            agent.Instance.transform.position) <= cellDist * HexGrid.HexDistance
                        && HexManager.UnitCurrentCell[color].unit.Color != agent.Color)
                    {
                        res.Add((Vector3.Distance(HexManager.UnitCurrentCell[color].unit.Instance.transform.position,
                            agent.Instance.transform.position), HexManager.UnitCurrentCell[color].unit));
                    }
                }

                return res.Count > 0 ? res.OrderBy(x => x.Item1).First().unit : null;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + " " + agent.Color + " ");
                return null;
            }
        }


        public BotState GetNewBehaviour(AIAgent agent)
        {
            var attack = agent.Unit.Inventory.Where(x => x is Bonus { BonusType: BonusType.Attack }).ToList();
            if (agent.CurentState is BotState.Attack && agent.Unit.AttackBonus == 0 && attack.Count > 0)
            {
                SetBehaviour(BotState.AttackBonusUsage, agent);
                return BotState.AttackBonusUsage;
            }


            var enemy = GetNearestUnit(_data.DistanceToAgr, agent.Unit);
            if (enemy != null && agent.Unit.Hp > agent.Unit.Data.maxHP * _data.PercentToRetreet && enemy.IsAlive)
            {
                if (agent.Unit.Hp <= agent.Unit.Data.maxHP * _data.PercentToRetreet ||
                    agent.Unit.UnitView.AvailableShots == 0)
                {
                    SetBehaviour(BotState.Retreet, agent);
                    return BotState.Retreet;
                }

                if (Vector3.Distance(agent.Unit.Instance.transform.position, enemy.Instance.transform.position) <=
                    agent.Unit.Weapon.disnatce)
                {
                    SetBehaviour(BotState.Attack, agent);
                    return BotState.Attack;
                }

                SetBehaviour(BotState.Agressive, agent);
                return BotState.Agressive;
            }

            var item = GetNearestItem(agent);
            if (((item.dist > 0 && item.dist <= _data.DistaceToCollectBonus) ||
                 agent.Unit.Mana <= agent.Unit.Data.maxMana * _data.ManaPercentToCollectBonus) &&
                (item.hex.Item.Type == ItemType.DEFENCE
                    ? agent.Unit.InventoryDefence.Count
                    : agent.Unit.Inventory.Count) < agent.Unit.InventoryCapacity / 2)
            {
                SetBehaviour(BotState.CollectingBonus, agent);
                return BotState.CollectingBonus;
            }

            var protect = agent.Unit.InventoryDefence.Where(x => x is Bonus { BonusType: BonusType.Defence }).ToList();
            if (protect.Count > 0 && agent.Unit.Hp <= agent.Unit.Data.maxHP * _data.PercentToUseProtectBonus &&
                agent.Unit.DefenceBonus == 0)
            {
                SetBehaviour(BotState.ProtectBonusUsage, agent);
                return BotState.ProtectBonusUsage;
            }


            SetBehaviour(BotState.Patrol, agent);
            return BotState.Patrol;
        }

        private void SetBehaviour(BotState state, AIAgent agent)
        {
            switch (state)
            {
                case BotState.Patrol:
                    StartPatrolBehaviour(agent);
                    break;
                case BotState.Agressive:
                    MoveToEnemy(agent);
                    break;
                case BotState.Attack:
                    AttackEnemy(agent);
                    break;
                case BotState.CollectingBonus:
                    MoveToBonus(agent);
                    break;
                case BotState.ProtectBonusUsage:
                    UseBonus(agent, BonusType.Defence);
                    break;
                case BotState.AttackBonusUsage:
                    UseBonus(agent, BonusType.Attack);
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

        private void UseBonus(AIAgent agent, BonusType type)
        {
            var attack = agent.Unit.Inventory.Where(x => x is Bonus bonus && bonus.BonusType == type).ToList();
            if (attack.Count == 0 || !agent.Unit.IsAlive)
            {
                GetNewBehaviour(agent);
                return;
            }

            ((Bonus)attack.First()).Invoke();
        }

        private void Retreet(AIAgent agent)
        {
            var enemy = GetNearestUnit(6, agent.Unit)?.Instance.transform;
            if (enemy == null)
            {
                return;
            }

            var dir = -DirectionHelper.DirectionTo(agent.Unit.Instance.transform.position,
                enemy.position);
            agent.currentPath.Clear();
            agent.currentPath.Enqueue(DirectionHelper.VectorToDirection(new Vector2(dir.x, dir.z)));
        }

        private (int dist, HexCell hex) GetNearestItem(AIAgent agent)
        {
            var itemsToMove =
                (from entry in ItemFabric.Items
                    where Vector3.Distance(agent.Unit.Instance.transform.position, entry.Value.transform.position) <
                          10 * HexGrid.HexDistance
                    orderby Vector3.Distance(agent.Unit.Instance.transform.position, entry.Value.transform.position)
                    select entry).ToList();


            if (itemsToMove.Count == 0)
            {
                return (0, null);
            }

            var itemToMove = itemsToMove.First();
            return (
                (int)(Vector3.Distance(itemToMove.Value.transform.position, agent.Unit.Instance.transform.position) /
                      HexGrid.HexDistance), itemToMove.Value);
        }

        private void MoveToBonus(AIAgent agent)
        {
            if (HexManager.UnitCurrentCell.TryGetValue(agent.Unit.Color, out var value))
                
                Pathfinding.FindPath(value.cell, GetNearestItem(agent).hex,
                    agent.currentPath);
        }

        private void AttackEnemy(AIAgent agent)
        {
            var enemy = GetNearestUnit(agent.Unit.Weapon.disnatce, agent.Unit);
            var dir = DirectionHelper.DirectionTo(agent.Unit.Instance.transform.position,
                enemy.Instance.transform.position);
            agent.AttackTarget(new Vector2(dir.x, dir.z));
        }

        private void MoveToEnemy(AIAgent agent)
        {
            var enemies = HexManager.UnitCurrentCell
                .Where(unit =>
                    unit.Value.unit.Color != agent.Unit.Color &&
                    Vector3.Distance(unit.Value.unit.Instance.transform.position,
                        agent.Unit.Instance.transform.position) <= 6 * HexGrid.HexDistance).ToList();

            Pathfinding.FindPath(HexManager.UnitCurrentCell[agent.Unit.Color].cell,
                enemies[Random.Range(0, enemies.Count)].Value.cell, agent.currentPath);
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
        Dead,
        Retreet
    }
}