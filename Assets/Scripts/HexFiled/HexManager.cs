using System;
using System.Collections.Generic;
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

        public static void GetNearestDifferCell(UnitColor color, Queue<HexCell> path)
        {
            HexCell end = UnitCurrentCell[color].cell;
            var itters = 0;
            while (end.Color == color)
            {
                var tmp = end;
                do
                {
                    end = tmp.Neighbors[Random.Range(0, 6)];
                    itters++;
                } while (end == null && itters < 5);

                if (itters >= 5)
                {
                    return;
                }
                path.Enqueue(end);
            }
            
        }

        public static void PaintHexList(List<HexCell> field, UnitColor color)
        {

            List<Action<UnitColor>> actions = new List<Action<UnitColor>>();

            field.ForEach(x => actions.Add(x.PaintHex));

            TimerHelper.Instance.StartTimer(actions, 0.05f, color);

        }
        
       
    }
}