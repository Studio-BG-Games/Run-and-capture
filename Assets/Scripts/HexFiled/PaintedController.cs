using System.Collections.Generic;
using Data;
using UnityEngine;

namespace HexFiled
{
    public class PaintedController
    {
        public static Dictionary<UnitColor, (HexCell previos, HexCell curent)> UnitCurrentCell;
        private HexCell _cell;

        public PaintedController()
        {
            UnitCurrentCell = new Dictionary<UnitColor, (HexCell, HexCell)>();
        }

        public void SetHexColors(HexCell cell)
        {
            _cell = cell;
            List<HexCell> cells = new List<HexCell>();

            for (int i = 0; i < 6; i++)
            {
                cells.Add(cell.GetNeighbor((HexDirection)i));
            }

            var hexByColorDict = DifferentHexByColor(cells);
            foreach (var item in hexByColorDict)
            {
                if (item.Value.Count > 0 && item.Key != UnitColor.GREY && item.Key != cell.Color)
                {
                    foreach (var cellNeighbour in item.Value)
                    {
                        if (UnitCurrentCell.ContainsKey(item.Key))
                        {
                            var path = HasPath(cellNeighbour, UnitCurrentCell[item.Key].curent);
                            if (!path.hasPath)
                            {
                                path.field.ForEach(x => x.PaintHex(UnitColor.GREY));
                            }
                        }
                    }
                }
                if (item.Key == cell.Color && item.Value.Count >= 2 && item.Value.Count < 6 &&
                         UnitCurrentCell.ContainsKey(cell.Color))
                {
                    HexDirection direction = new HexDirection();
                    HexDirection openDirection = new HexDirection();
                    HexDirection closeDirection = new HexDirection();
                    for (int i = 0; i < 6; i++)
                    {
                        var neighbour = UnitCurrentCell[cell.Color].previos.GetNeighbor((HexDirection)i);
                        
                        if (neighbour == UnitCurrentCell[cell.Color].curent)
                        {
                            direction = (HexDirection)i;
                        }
                    }
                    
                    openDirection = direction.PlusSixtyDeg();
                    closeDirection = direction.MinusSixtyDeg();
                    

                    var path = HasPath(
                        UnitCurrentCell[cell.Color].previos.GetNeighbor(closeDirection),
                        UnitCurrentCell[cell.Color].previos.GetNeighbor(openDirection)
                        );

                    if (!path.hasPath)
                    {
                        path.field.ForEach(x => x.PaintHex(cell.Color));
                    }
                    else
                    {
                        path = HasPath(
                            UnitCurrentCell[cell.Color].previos.GetNeighbor(openDirection),
                            UnitCurrentCell[cell.Color].previos.GetNeighbor(closeDirection)
                        );
                        if (!path.hasPath)
                        {
                            path.field.ForEach(x => x.PaintHex(cell.Color));
                        }
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

        private ( bool hasPath, List<HexCell> field ) HasPath(HexCell start, HexCell end)
        {
            List<HexCell> closedList = new List<HexCell>();
            HexCell currentCell = start;

            Stack<HexCell> stackIteators = new Stack<HexCell>();
            stackIteators.Push(currentCell);

            closedList.Add(currentCell);


            while (stackIteators.Count >= 0)
            {
                if (currentCell == end)
                    return (true, closedList);

                List<HexCell> openList = new List<HexCell>();
                if(end.Color != UnitColor.GREY)
                {
                    if (end.Color != start.Color)
                    {
                        return (true, null);
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        HexCell neighbour = currentCell.GetNeighbor((HexDirection)i);
                        if (neighbour == null)
                        {
                            return (true, null);
                        }

                        if (!closedList.Contains(neighbour) && neighbour.Color == start.Color)
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
                else
                {
                   
                    for (int i = 0; i < 6; i++)
                    {
                        HexCell neighbour = currentCell.GetNeighbor((HexDirection)i);
                        if (neighbour == null)
                        {
                            return (true, null);
                        }

                        if (!closedList.Contains(neighbour) && 
                            neighbour.Color != _cell.Color)
                        {
                            openList.Add(neighbour);
                        }
                    }
                }

                if (openList.Count > 0)
                {
                    currentCell = openList[Random.Range(0, openList.Count - 1)];
                    closedList.Add(currentCell);
                    stackIteators.Push(currentCell);
                }
                else
                {
                    if (stackIteators.Count == 0)
                    {
                        return (false, closedList);
                    }

                    currentCell = stackIteators.Pop();
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