using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using DefaultNamespace;
using DefaultNamespace.AI;
using HexFiled;
using Items;
using Units;
using Units.Wariors.AbstractsBase;
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
            HexManager.agents = new Dictionary<GameObject, AIBase>();
        }

        public void AddAgent(AIBase agent)
        {
            agent.OnAgentInited += InitAI;
        }

        public void RemoveAgent(AIBase agent)
        {
            agent.OnAgentInited -= InitAI;
        }

        private void InitAI(AIBase agent)
        {
            SetBehaviour(BotState.Patrol, agent);
        }

        private void StartPatrolBehaviour(AIBase agent)
        {
            HexManager.GetNearestDifferCell(agent.UnitBase.Color, agent.currentPath);
            while (agent.currentPath.Count == 0 && _triesToCalculatePath < _maxTriesToCalculatePath)
            {
                HexManager.GetNearestDifferCell(agent.UnitBase.Color, agent.currentPath);
                _triesToCalculatePath++;
            }

            _triesToCalculatePath = 0;
        }

        public static UnitBase GetNearestUnit(int cellDist, UnitBase agent)
        {
            List<(float dist, UnitBase unit)> res = new List<(float, UnitBase)>();
            try
            {
                
                res.AddRange(from color in (UnitColor[]) Enum.GetValues(typeof(UnitColor))
                    where HexManager.UnitCurrentCell.ContainsKey(color) &&
                          HexManager.UnitCurrentCell[color].unit.IsVisible &&
                          HexManager.UnitCurrentCell[color] != (null, null) &&
                          Vector3.Distance(HexManager.UnitCurrentCell[color].unit.Instance.transform.position,
                              agent.Instance.transform.position) <= cellDist * HexGrid.HexDistance &&
                          HexManager.UnitCurrentCell[color].unit.Color != agent.Color
                    select (
                        Vector3.Distance(HexManager.UnitCurrentCell[color].unit.Instance.transform.position,
                            agent.Instance.transform.position), HexManager.UnitCurrentCell[color].unit));

                return res.Count > 0 ? res.OrderBy(x => x.Item1).First().unit : null;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + " " + agent.Color + " ");
                return null;
            }
        }


        public BotState GetNewBehaviour(AIBase agent)
        {
            var attack = agent.UnitBase.Inventory.Where(x => x.Item is Bonus { BonusType: BonusType.Attack }).ToList();
            if (agent.CurentState is BotState.Attack && agent.UnitBase.AttackBonus == 0 && attack.Count > 0)
            {
                SetBehaviour(BotState.AttackBonusUsage, agent);
                return BotState.AttackBonusUsage;
            }


            var enemy = GetNearestUnit(_data.DistanceToAgr, agent.UnitBase);
            if (enemy != null && agent.UnitBase.Hp > agent.UnitBase.maxHP * _data.PercentToRetreet && enemy.IsAlive)
            {
                if (agent.UnitBase.Hp <= agent.UnitBase.maxHP * _data.PercentToRetreet ||
                    agent.UnitBase.BaseView.AvailableShots == 0)
                {
                    SetBehaviour(BotState.Retreet, agent);
                    return BotState.Retreet;
                }

                if (Vector3.Distance(agent.UnitBase.Instance.transform.position, enemy.Instance.transform.position) <=
                    agent.UnitBase.Weapon.disnatce)
                {
                    SetBehaviour(BotState.Attack, agent);
                    return BotState.Attack;
                }

                SetBehaviour(BotState.Agressive, agent);
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
                        SetBehaviour(BotState.CollectingBonus, agent);
                        return BotState.CollectingBonus;
                    }
                }


                var protect = agent.UnitBase.InventoryDefence.Where(x => x.Item is Bonus { BonusType: BonusType.Defence })
                    .ToList();
                if (protect.Count > 0 && agent.UnitBase.Hp <= agent.UnitBase.maxHP * _data.PercentToUseProtectBonus &&
                    agent.UnitBase.DefenceBonus == 0)
                {
                    SetBehaviour(BotState.ProtectBonusUsage, agent);
                    return BotState.ProtectBonusUsage;
                }
            

            SetBehaviour(BotState.Patrol, agent);
            return BotState.Patrol;
        }

        private void SetBehaviour(BotState state, AIBase agent)
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
                    if (agent != (AIAgent)agent) break;
                    MoveToBonus((AIAgent)agent);
                    break;
                case BotState.ProtectBonusUsage:
                    if (agent != (AIAgent)agent) break;
                    UseBonus((AIAgent)agent, BonusType.Defence);
                    break;
                case BotState.AttackBonusUsage:
                    if (agent != (AIAgent)agent) break;
                    UseBonus((AIAgent)agent, BonusType.Attack);
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
            var attack = agent.UnitBase.Inventory.Where(x => x.Item is Bonus bonus && bonus.BonusType == type).ToList();
            if (attack.Count == 0 || !agent.UnitBase.IsAlive)
            {
                GetNewBehaviour(agent);
                return;
            }

            ((Bonus)attack.First().Item).Invoke((Unit)(agent.UnitBase));
        }

        private void Retreet(AIBase agent)
        {
           var enemy = GetNearestUnit(6, agent.UnitBase)?.Instance.transform;

            if (enemy == null)
            {
                return;
            }

            var dir = -DirectionHelper.DirectionTo(agent.UnitBase.Instance.transform.position,
                enemy.position);
            agent.currentPath.Clear();
            agent.currentPath.Enqueue(DirectionHelper.VectorToDirection(new Vector2(dir.x, dir.z)));
        }

        private (int dist, HexCell hex) GetNearestItem(AIBase agent)
        {
            var itemsToMove =
                (from entry in ItemFabric.Items
                    where Vector3.Distance(agent.UnitBase.Instance.transform.position, entry.Value.transform.position) <
                          10 * HexGrid.HexDistance
                    orderby Vector3.Distance(agent.UnitBase.Instance.transform.position, entry.Value.transform.position)
                    select entry).ToList();


            if (itemsToMove.Count == 0)
            {
                return (0, null);
            }

            var itemToMove = itemsToMove.First();
            return (
                (int)(Vector3.Distance(itemToMove.Value.transform.position, agent.UnitBase.Instance.transform.position) /
                      HexGrid.HexDistance), itemToMove.Value);
        }

        private void MoveToBonus(AIAgent agent)
        {
            if (HexManager.UnitCurrentCell.TryGetValue(agent.UnitBase.Color, out var value))

                Pathfinding.FindPath(value.cell, GetNearestItem(agent).hex,
                    agent.currentPath);
        }

        private void AttackEnemy(AIBase agent)
        {
            var enemy = GetNearestUnit(agent.UnitBase.Weapon.disnatce, agent.UnitBase);
            var dir = DirectionHelper.DirectionTo(agent.UnitBase.Instance.transform.position,
                enemy.Instance.transform.position);
            agent.AttackTarget(new Vector2(dir.x, dir.z));
        }

        private void MoveToEnemy(AIBase agent)
        {
            var enemies = HexManager.UnitCurrentCell.Where(unit =>
                    unit.Value.unit.Color != agent.UnitBase.Color &&
                    Vector3.Distance(unit.Value.unit.Instance.transform.position,
                        agent.UnitBase.Instance.transform.position) <= 6 * HexGrid.HexDistance).ToList();
            if (enemies[Random.Range(0, enemies.Count)].Value.unit.Color == agent.UnitBase.Color) return;

            Pathfinding.FindPath(HexManager.UnitCurrentCell[agent.UnitBase.Color].cell,
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