using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using DefaultNamespace;
using DefaultNamespace.AI;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexFiled
{
    public static class HexManager
    {
        public static Dictionary<UnitColor, (HexCell cell, Unit unit)> UnitCurrentCell;
        public static Dictionary<UnitColor, List<HexCell>> CellByColor;
        public static Dictionary<GameObject, AIAgent> agents;

        public static void GetNearestDifferCell(UnitColor color, Queue<HexDirection> path)
        {
            if (!UnitCurrentCell.TryGetValue(color, out var unit))
            {
                return;
            }
            HexCell end = unit.cell;
            var itters = 0;
            var neighbours = end.GetListNeighbours().Where(cell => cell != null && cell.Color != color).ToList();
            if (neighbours.Any())
            {
                var dir = DirectionHelper.DirectionTo(end.transform.position,
                    neighbours[Random.Range(0, neighbours.Count - 1)].transform.position);
                path.Enqueue(DirectionHelper.VectorToDirection(new Vector2(dir.x, dir.z)));
                return;
            }
            
            while (end.Color == color)
            {
                var tmp = end;
                HexDirection dir;
                do
                {
                    dir = (HexDirection)Random.Range(0, 6);
                    end = tmp.GetNeighbor(dir);
                    itters++;
                } while (end == null && itters < 5);

                if (itters >= 3)
                {
                    return;
                }
                path.Enqueue(dir);
            }
            
        }

        public static void PaintHexList(List<HexCell> field, UnitColor color, float time)
        {

            List<Action<UnitColor, bool>> actions = new List<Action<UnitColor, bool>>();

            field.ForEach(x => actions.Add(x.PaintHex));

            TimerHelper.Instance.StartTimer(actions, time, color);

        }

        public static void PaintHexList(List<HexCell> field, UnitColor color)
        {
            List<HexCell> cells = new List<HexCell>();
            
            cells.AddRange(field);
            
            cells.ForEach(x => x.PaintHex(color, true));
        }
    }
}