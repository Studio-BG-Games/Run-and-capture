using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Controller;
using DefaultNamespace;
using DefaultNamespace.AI;
using HexFiled;
using Items;
using Runtime.Controller;
using Units;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AI
{
    public class AIManager
    {
        private List<AIAgent> _agents;
        private int _triesToCalculatePath = 0;
        private int _maxTriesToCalculatePath = 5;

        private static AIManager _instance;


        public static AIManager Instance
        {
            get => _instance;
            private set => _instance ??= value;
        }

        public AIManager(List<AIAgent> agents)
        {
            _agents = agents;

            agents.ForEach(agent => { SetBehaviour(BotState.Patrol, agent); });
        }

        public AIManager()
        {
            _agents = new List<AIAgent>();

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
            SetBehaviour(BotState.Patrol, agent);
        }

        private void StartPatrolBehaviour(AIAgent agent)
        {
            HexManager.GetNearestDifferCell(agent.Enemy.Color, agent.currentPath);
            while (agent.currentPath.Count == 0 && _triesToCalculatePath < _maxTriesToCalculatePath)
            {
                HexManager.GetNearestDifferCell(agent.Enemy.Color, agent.currentPath);
                _triesToCalculatePath++;
            }

            _triesToCalculatePath = 0;
        }

        private Unit GetNearestUnit(int cellDist, AIAgent agent)
        {
            return (from unit in HexManager.UnitCurrentCell
                where unit.Key != agent.Enemy.Color &&
                      Vector3.Distance(unit.Value.unit.Instance.transform.position,
                          agent.Enemy.Instance.transform.position) <= cellDist * HexGrid.HexDistance
                select unit.Value.unit).FirstOrDefault();
        }

        public BotState GetNewBehaviour(AIAgent agent)
        {
            var attack = agent.Enemy.Inventory.Where(x => x is Bonus { Type: BonusType.Attack }).ToList();
            if (agent.CurentState is BotState.Attack && agent.Enemy.AttackBonus == 0 && attack.Count > 0)
            {
                SetBehaviour(BotState.AttackBonusUsage, agent);
                return BotState.AttackBonusUsage;
            }

            var enemy = GetNearestUnit(6, agent);
            if (enemy != null && agent.Enemy.Hp > agent.Enemy.Data.maxHP / 4 && enemy.IsAlive)
            {
                if (Vector3.Distance(agent.Enemy.Instance.transform.position, enemy.Instance.transform.position) < 4)
                {
                    SetBehaviour(BotState.Attack, agent);
                    return BotState.Attack;
                }

                SetBehaviour(BotState.Agressive, agent);
                return BotState.Agressive;
            }

            if (agent.Enemy.Mana <= agent.Enemy.Data.maxMana / 3 &&
                agent.Enemy.Inventory.Count < agent.Enemy.InventoryCapacity)
            {
                SetBehaviour(BotState.CollectingBonus, agent);
                return BotState.CollectingBonus;
            }

            var protect = agent.Enemy.Inventory.Where(x => x is Bonus { Type: BonusType.Defence }).ToList();
            if (protect.Count > 0 && agent.Enemy.Hp <= agent.Enemy.Data.maxHP / 4 && agent.Enemy.DefenceBonus == 0)
            {
                SetBehaviour(BotState.ProtectBonusUsage, agent);
                return BotState.ProtectBonusUsage;
            }

            if (agent.Enemy.Hp <= agent.Enemy.Data.maxHP / 4 && GetNearestUnit(5, agent) != null)
            {
                SetBehaviour(BotState.Retreet, agent);
                return BotState.Retreet;
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
            var attack = agent.Enemy.Inventory.Where(x => x is Bonus bonus && bonus.Type == type).ToList();
            if (attack.Count == 0)
            {
                GetNewBehaviour(agent);
                return;
            }

            ((Bonus)attack.First()).Invoke();
        }

        private void Retreet(AIAgent agent)
        {
            var enemy = GetNearestUnit(6, agent)?.Instance.transform;
            if (enemy == null)
            {
                return;
            }

            var dir = -DirectionHelper.DirectionTo(agent.Enemy.Instance.transform.position,
                enemy.position);
            agent.currentPath.Clear();
            agent.currentPath.Enqueue(DirectionHelper.VectorToDirection(new Vector2(dir.x, dir.z)));
        }

        private void MoveToBonus(AIAgent agent)
        {
            HexCell itemToMove = null;
            var min = 10 * HexGrid.HexDistance;

            foreach (var itemCell in ItemFabric.Items)
            {
                if (Vector3.Distance(agent.Enemy.Instance.transform.position, itemCell.Key.transform.position) < min)
                {
                    min = Vector3.Distance(agent.Enemy.Instance.transform.position, itemCell.Key.transform.position);
                    itemToMove = itemCell.Value;
                }
            }

            if (itemToMove == null)
            {
                return;
            }

            Pathfinding.FindPath(HexManager.UnitCurrentCell[agent.Enemy.Color].cell, itemToMove, agent.currentPath);
        }

        private void AttackEnemy(AIAgent agent)
        {
            var enemy = GetNearestUnit(3, agent);
            var dir = DirectionHelper.DirectionTo(agent.Enemy.Instance.transform.position,
                enemy.Instance.transform.position);
            agent.AttackTarget(new Vector2(dir.x, dir.z));
        }

        private void MoveToEnemy(AIAgent agent)
        {
            var enemies = HexManager.UnitCurrentCell
                .Where(unit =>
                    unit.Value.unit.Color != agent.Enemy.Color &&
                    Vector3.Distance(unit.Value.unit.Instance.transform.position,
                        agent.Enemy.Instance.transform.position) <= 6 * HexGrid.HexDistance).ToList();

            Pathfinding.FindPath(HexManager.UnitCurrentCell[agent.Enemy.Color].cell,
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