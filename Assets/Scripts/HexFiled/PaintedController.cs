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

        public void CheckDeath(HexCell cell)
        {
            foreach (var cells in HexManager.UnitCurrentCell.Where(cells => HexManager.CellByColor[cells.Key].Count < 2 || (cells.Value.cell == cell && cells.Value.unit.Color != cell.Color)))
            {
                cells.Value.unit.Death();
            }
        }
        public void SetHexColors(HexCell cell)
        {
            _cell = cell;
            var cells = new List<HexCell>();

            for (var i = 0; i < 6; i++)
            {
                cells.Add(cell.GetNeighbor((HexDirection)i));
            }

            var hexByColorDict = DifferentHexByColor(cells);
            foreach (var item in hexByColorDict)
            {
                if (item.Key == cell.Color && item.Value.Count >= 2 && item.Value.Count < 6 &&
                    HexManager.UnitCurrentCell.ContainsKey(cell.Color))
                {
                    
                    cell.GetListNeighbours().ForEach(x =>
                    {
                        if (x != null && x.Color != cell.Color)
                        {
                            var path = Round(x, null);
                            if(!path.hasPath)
                                HexManager.PaintHexList(path.field, cell.Color);
                        }
                    });
                }

                if (item.Value.Count > 0 && item.Key != UnitColor.GREY && item.Key != cell.Color)
                {
                    foreach (var path in from cellNeighbour in item.Value
                             where HexManager.UnitCurrentCell.ContainsKey(item.Key)
                             select HasPath(cellNeighbour, HexManager.UnitCurrentCell[item.Key].cell)
                             into path
                             where !path.hasPath
                             select path)
                    {
                        if(!path.hasPath)
                            HexManager.PaintHexList(path.field, UnitColor.GREY);
                    }
                }
            }
        }


       

        private Dictionary<UnitColor, List<HexCell>> DifferentHexByColor(List<HexCell> cellsList)
        {
            Dictionary<UnitColor, List<HexCell>> resultDict = new Dictionary<UnitColor, List<HexCell>>();
            cellsList.ForEach(cell =>
            {
                if (cell != null && resultDict.ContainsKey(cell.Color))
                {
                    resultDict[cell.Color].Add(cell);
                }
                else if (cell != null)
                {
                    resultDict.Add(cell.Color, new List<HexCell> { cell });
                }
            });
            return resultDict;
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

        private ( bool hasPath, List<HexCell> field ) HasPath(HexCell start, HexCell end)
        {
            if (start.Color == _cell.Color || end.Color == _cell.Color)
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
                if (currentCell == end)
                    return (true, null);

                List<HexCell> openList = new List<HexCell>();

                foreach (var neighbour in currentCell.GetListNeighbours())
                {
                    if (neighbour == null)
                    {
                        return (true, null);
                    }


                    if (closedList.Contains(neighbour) || neighbour.Color != start.Color) continue;
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
    }
}