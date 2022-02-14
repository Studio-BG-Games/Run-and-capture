using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


        public async void SetHexColors(HexCell cell)
        {
            _cell = cell;


            var hexByColorDict = Enum.GetValues(typeof(UnitColor)).Cast<UnitColor>().ToDictionary(color => color,
                color => cell.GetListNeighbours().Where(x => x != null && x.Color == color).ToList());


            var neighbours = cell.GetListNeighbours().Where(x => x != null && x.Color != cell.Color).ToArray();

            foreach (var neighbour in neighbours)
            {
                if (hexByColorDict.TryGetValue(neighbour.Color, out var value) &&
                    hexByColorDict.TryGetValue(cell.Color, out var hexCells) &&
                    hexCells.Count >= 2 &&
                    value.Count < 6)
                {
                    foreach (var hex in value)
                    {
                        
                        var path = await Round(hex, null);
                        if (!path.hasPath)
                        {
                            HexManager.PaintHexList(path.field, cell.Color, 0.005f);
                        }
                    }
                }


                if (neighbour.Color != UnitColor.Grey
                    && HexManager.UnitCurrentCell.TryGetValue(neighbour.Color, out var unit)
                    && !unit.unit.IsStaned
                    && hexByColorDict.TryGetValue(neighbour.Color, out var cells)
                    && cells.Count >= 2 && cells.Count < 6)
                {
                    var path = await HasPath(neighbour, unit.cell);
                    if (!path.hasPath)
                        HexManager.PaintHexList(path.field, UnitColor.Grey);
                }
            }
        }


        private async Task<(bool hasPath, List<HexCell> field)> Round(HexCell start, HexCell end)
        {
            if (start == null || start.Color == _cell.Color)
            {
                await Task.CompletedTask;
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
                        await Task.CompletedTask;
                        return (true, null);
                    }

                    if (closedList.Contains(neighbour) || neighbour.Color == _cell.Color) continue;
                    openList.Add(neighbour);
                    if (neighbour.GetListNeighbours().Contains(end))
                    {
                        await Task.CompletedTask;
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
                        await Task.CompletedTask;
                        return (false, closedList);
                    }

                    currentCell = stackIterators.Pop();
                }

                if (currentCell.GetListNeighbours().Contains(end))
                {
                    await Task.CompletedTask;
                    return (true, null);
                }
            }

            await Task.CompletedTask;
            return (false, closedList);
        }

        private async Task<(bool hasPath, List<HexCell> field)> HasPath(HexCell start, HexCell end)
        {
            if (start.Color == _cell.Color)
            {
                await Task.CompletedTask;
                return (true, null);
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
                    await Task.CompletedTask;
                    return (true, null);
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
                        await Task.CompletedTask;
                        return (false, closedList);
                    }

                    currentCell = stackIterators.Pop();
                }

                if (currentCell.GetListNeighbours().Contains(end))
                {
                    await Task.CompletedTask;
                    return (true, null);
                }
            }

            await Task.CompletedTask;
            return (false, closedList);
        }
    }
}