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
    public abstract class AIManager
        {
            protected int _triesToCalculatePath = 0;
            protected int _maxTriesToCalculatePath = 5;
            protected AIData _data;

            protected static AIManager _instance;


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
            protected abstract void SetBehaviour(BotState state, AIBase agent, int dist);
            protected abstract void InitAI(AIBase agent);

            protected void StartPatrolBehaviour(AIBase agent)
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

                    res.AddRange(from color in (UnitColor[])Enum.GetValues(typeof(UnitColor))
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


            public abstract BotState GetNewBehaviour(AIBase agent);

            protected void Retreet(AIBase agent)
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

            protected (int dist, HexCell hex) GetNearestItem(AIBase agent)
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

            protected void MoveToBonus(AIBase agent)
            {
                if (HexManager.UnitCurrentCell.TryGetValue(agent.UnitBase.Color, out var value))

                    Pathfinding.FindPath(value.cell, GetNearestItem(agent).hex,
                        agent.currentPath);
            }

            protected void AttackEnemy(AIBase agent)
            {
                var enemy = GetNearestUnit(agent.UnitBase.Weapon.disnatce, agent.UnitBase);
                var dir = DirectionHelper.DirectionTo(agent.UnitBase.Instance.transform.position,
                    enemy.Instance.transform.position);
                agent.AttackTarget(new Vector2(dir.x, dir.z));
            }

            protected void MoveToEnemy(AIBase agent, int dist)
            {
                var enemies = HexManager.UnitCurrentCell.Where(unit =>
                        unit.Value.unit.Color != agent.UnitBase.Color &&
                        Vector3.Distance(unit.Value.unit.Instance.transform.position,
                            agent.UnitBase.Instance.transform.position) <= dist * HexGrid.HexDistance).ToList();
                if (enemies[Random.Range(0, enemies.Count)].Value.unit.Color == agent.UnitBase.Color) return;

                Pathfinding.FindPath(HexManager.UnitCurrentCell[agent.UnitBase.Color].cell,
                    enemies[Random.Range(0, enemies.Count)].Value.cell, agent.currentPath);
            }
            protected void CatchHex(AIBase agent)
            {
                Pathfinding.FindPath(HexManager.UnitCurrentCell[agent.UnitBase.Color].cell, HexManager.CellByColor[UnitColor.Grey].Where(x => x != null).ToList()[
      Random.Range(0, HexManager.CellByColor[UnitColor.Grey].Count - 1)]
        , agent.currentPath);
            }

        protected void UseBonus(AIAgent agent, BonusType type)
        {
            var attack = agent.UnitBase.Inventory.Where(x => x.Item is Bonus bonus && bonus.BonusType == type).ToList();
            if (attack.Count == 0 || !agent.UnitBase.IsAlive)
            {
                GetNewBehaviour(agent);
                return;
            }

            ((Bonus)attack.First().Item).Invoke((Units.Unit.Unit)agent.UnitBase);
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