using System;
using System.Collections.Generic;
using System.Linq;
using Units;
using Random = UnityEngine.Random;

namespace HexFiled
{
    public class PaintedController
    {
        private HexCell _cell;

        public PaintedController()
        {
            HexManager.UnitCurrentCell = new Dictionary<UnitColor, (HexCell cell, Unit unit)>();
        }


        public void SetHexColors(HexCell cell)
        {
            _cell = cell;


            var hexByColorDict = Enum.GetValues(typeof(UnitColor)).Cast<UnitColor>().ToDictionary(color => color,
                color => cell.GetListNeighbours().Where(x => x != null && x.Color == color).ToList());

            cell.GetListNeighbours().Where(x => x != null && x.Color != cell.Color).ToList().ForEach(neighbour =>
            {
                if (hexByColorDict.TryGetValue(neighbour.Color, out var value) &&
                    value.Count >= 2 && value.Count < 6)
                {
                    value.ForEach(x =>
                    {
                        var path = Round(x, null);
                        if (!path.hasPath)
                        {
                            HexManager.PaintHexList(path.field, cell.Color, 0.05f);
                        }
                    });
                }


                if (neighbour.Color != UnitColor.Grey
                    && HexManager.UnitCurrentCell.TryGetValue(neighbour.Color, out var unit)
                    && hexByColorDict.TryGetValue(neighbour.Color, out var cells)
                    && cells.Count >= 2 && cells.Count < 5
                    && !HasPath(neighbour, unit.cell, out var path))
                {
                    HexManager.PaintHexList(path, UnitColor.Grey);
                }
            });
        }


        private (bool hasPath, List<HexCell> field) Round(HexCell start, HexCell end)
        {
            if (start == null || start.Color == _cell.Color)
            {
                return (true, null);
            }

            List<HexCell> closedList = new List<HexCell>();
            HexCell currentCell = start;

            Stack<HexCell> stackIterators = new Stack<HexCell>();
            stackIterators.Push(currentCell);

            closedList.Add(currentCell);

            while (stackIterators.Count >= 0)
            {
                if (end != null && currentCell == end)
                    return (true, closedList);

                List<HexCell> openList = new List<HexCell>();

                foreach (var neighbour in currentCell.GetListNeighbours())
                {
                    if (neighbour == null)
                    {
                        return (true, null);
                    }

                    if (closedList.Contains(neighbour) || neighbour.Color == _cell.Color) continue;
                    openList.Add(neighbour);
                    if (neighbour.GetListNeighbours().Contains(end))
                    {
                        return (true, null);
                    }
                }

                if (openList.Count > 0)
                {
                    currentCell = openList[Random.Range(0, openList.Count - 1)];
                    closedList.Add(currentCell);
                    stackIterators.Push(currentCell);
                }
                else
                {
                    if (stackIterators.Count == 0)
                    {
                        return (false, closedList);
                    }

                    currentCell = stackIterators.Pop();
                }

                if (currentCell.GetListNeighbours().Contains(end))
                {
                    return (true, null);
                }
            }

            return (false, closedList);
        }

        private bool HasPath(HexCell start, HexCell end,
            out List<HexCell> value)
        {
            if (start.Color == _cell.Color || end.Color == _cell.Color)
            {
                value = null;
                return true;
            }

            List<HexCell> closedList = new List<HexCell>();
            HexCell currentCell = start;

            Stack<HexCell> stackIterators = new Stack<HexCell>();
            stackIterators.Push(currentCell);

            closedList.Add(currentCell);

            while (stackIterators.Count >= 0)
            {
                if (currentCell == end)
                {
                    value = null;
                    return true;
                }

                List<HexCell> openList = currentCell.GetListNeighbours()
                    .Where(neighbour =>
                        neighbour != null && !closedList.Contains(neighbour) && neighbour.Color == start.Color)
                    .ToList();


                if (openList.Count > 0)
                {
                    currentCell = openList[Random.Range(0, openList.Count - 1)];
                    closedList.Add(currentCell);
                    stackIterators.Push(currentCell);
                }
                else
                {
                    if (stackIterators.Count == 0)
                    {
                        value = closedList;
                        return false;
                    }

                    currentCell = stackIterators.Pop();
                }

                if (currentCell.GetListNeighbours().Contains(end))
                {
                    value = null;
                    return true;
                }
            }

            value = closedList;
            return false;
        }
    }
}