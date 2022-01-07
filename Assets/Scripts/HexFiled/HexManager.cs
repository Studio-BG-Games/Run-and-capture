using System.Collections.Generic;
using AI;
using DefaultNamespace.AI;
using Units;
using UnityEngine;

namespace HexFiled
{
    public static class HexManager
    {
        public static Dictionary<UnitColor, (HexCell cell, Unit unit)> UnitCurrentCell;
        public static Dictionary<UnitColor, List<HexCell>> CellByColor;
        public static Dictionary<GameObject, AIAgent> agents;

        public static void GetNearestDifferCell(UnitColor color, Queue<HexCell> path)
        {
            HexCell end = UnitCurrentCell[color].cell;
            var itters = 0;
            while (end.Color == color)
            {
                var tmp = end;
                do
                {
                    end = tmp.Neighbors[Random.Range(0, 5)];
                    itters++;
                } while (end == null && itters < 5);

                if (itters >= 5)
                {
                    return;
                }
                path.Enqueue(end);
            }
            
        }
        
        // public  static ( bool hasPath, List<HexCell> field ) HasPath(UnitColor color)
        // {
        //     var start = UnitCurrentCell[color];
        //     var end = start;
        //     List<HexCell> neighboursCells = new List<HexCell>();
        //     while (end.Color == color)
        //     {
        //         neighboursCells.AddRange(end.Neighbors);
        //         neighboursCells.ForEach(cell =>
        //         {
        //             if (cell.Color != color)
        //             {
        //                 end = cell;
        //             }
        //         });
        //     }
        //
        //     List<HexCell> closedList = new List<HexCell>();
        //     HexCell currentCell = start;
        //
        //     Stack<HexCell> stackIterators = new Stack<HexCell>();
        //     stackIterators.Push(currentCell);
        //
        //     closedList.Add(currentCell);
        //
        //
        //     while (stackIterators.Count >= 0)
        //     {
        //         if (currentCell == end)
        //             return (true, null);
        //
        //         List<HexCell> openList = new List<HexCell>();
        //
        //         foreach (var neighbour in currentCell.GetListNeighbours())
        //         {
        //             if (neighbour == null)
        //             {
        //                 return (true, null);
        //             }
        //
        //
        //             if (closedList.Contains(neighbour) || neighbour.Color != start.Color) continue;
        //             openList.Add(neighbour);
        //             if (neighbour.GetListNeighbours().Contains(end))
        //             {
        //                 return (true, null);
        //             }
        //         }
        //
        //
        //         if (openList.Count > 0)
        //         {
        //             currentCell = openList[Random.Range(0, openList.Count - 1)];
        //             closedList.Add(currentCell);
        //             stackIterators.Push(currentCell);
        //         }
        //         else
        //         {
        //             if (stackIterators.Count == 0)
        //             {
        //                 return (false, closedList);
        //             }
        //
        //             currentCell = stackIterators.Pop();
        //         }
        //
        //         if (currentCell.GetListNeighbours().Contains(end))
        //         {
        //             return (true, null);
        //         }
        //     }
        //
        //     return (false, closedList);
        // }
    }
}