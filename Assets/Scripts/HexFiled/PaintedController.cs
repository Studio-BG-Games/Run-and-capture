using System.Collections.Generic;
using UnityEngine;

namespace HexFiled
{
   
    public class PaintedController
    {
        public static Dictionary<UnitColor, HexCell> unitCurrentCell = new Dictionary<UnitColor, HexCell>();

        private HexCell _cell;

        public void SetHexColors(HexCell cell)
        {
            _cell = cell;
            List<HexCell> cells = new List<HexCell>();

            for(int i = 0 ; i < 6 ; i++)
            {
                cells.Add(cell.GetNeighbor((HexDirection)i));
            }

            var hexByColorDict = DifferentHexByColor(cells);
            foreach (var item in hexByColorDict)
            {
                if(item.Value.Count > 0 && item.Key != UnitColor.GREY && item.Key != cell.Color )
                {
                    foreach(var cellNeighbour in item.Value)
                    {
                        if(unitCurrentCell.ContainsKey(item.Key))
                        {
                            var path = HasPath(cellNeighbour, unitCurrentCell[item.Key]);
                            if(!path.hasPath)
                            {
                                path.field.ForEach(x => x.PaintHex(UnitColor.GREY));
                            }
                        }
                    }
                }
                else if (item.Key == UnitColor.GREY)
                {
                    
                }
            }

        }

        private Dictionary<UnitColor, List<HexCell>> DifferentHexByColor(List<HexCell> cellsList)
        {
            Dictionary<UnitColor, List<HexCell>> resultDict = new Dictionary<UnitColor, List<HexCell>>();
            cellsList.ForEach(cell => {

                if(cell != null && resultDict.ContainsKey(cell.Color))
                {
                    resultDict[cell.Color].Add(cell);
                }
                else if(cell != null)
                {
                    resultDict.Add(cell.Color, new List<HexCell>{cell});
                }
                
            } );
            return resultDict;
        }

        private ( bool hasPath , List<HexCell> field ) HasPath(HexCell start, HexCell end)
        {
            List<HexCell> closedList = new List<HexCell>();
            HexCell currentCell = start;

            Stack<HexCell> stackIteators = new Stack<HexCell>();
            stackIteators.Push(currentCell);

            closedList.Add(currentCell);



            while(stackIteators.Count >= 0 )
            {
                if(currentCell == end)
                    return ( true,  closedList);

                List<HexCell> openList = new List<HexCell>();
                for(int i = 0 ; i < 6; i++)
                {
                    HexCell neighbour = currentCell.GetNeighbor( (HexDirection)i );
                    if(!closedList.Contains(neighbour) && neighbour.Color == end.Color)
                    {
                        openList.Add(neighbour);
                    }
                }

                if(openList.Count > 0)
                {
                    currentCell = openList[Random.Range(0, openList.Count -1 )];
                    closedList.Add(currentCell);
                    stackIteators.Push(currentCell);
                }
                else
                {
                    if (stackIteators.Count == 0)
                    {
                        return ( false,   closedList);
                    }
                    currentCell = stackIteators.Pop();
                }
            }
            return ( false,   closedList);

        }
    }
}
/*
                if(start == end)
                    return true;
                    
                if(currentCell.Cell.coordinates.Y < end.coordinates.Y)
                {
                    var cellNeighbour = currentCell.Cell.GetNeighbor(HexDirection.W);
                    if (!closedList.Contains(cellNeighbour) && cellNeighbour.Color == end.Color)
                    {
                        stackIteators.Push(currentCell);

                        currentCell = new HexIterator(cellNeighbour, true, currentCell.Cell);

                        closedList.Add(currentCell.Cell);
                        continue;
                    }
                }
                else if(currentCell.Cell.coordinates.Y > end.coordinates.Y)
                {
                    var cellNeighbour = currentCell.Cell.GetNeighbor(HexDirection.E);
                    if(!closedList.Contains(cellNeighbour) && cellNeighbour.Color == end.Color)
                    {
                        stackIteators.Push(currentCell);
                        currentCell = new HexIterator(cellNeighbour, true, currentCell.Cell);

                        closedList.Add(currentCell.Cell);
                        continue;
                    }
                }
                // Z

                if(currentCell.Cell.coordinates.Z > end.coordinates.Z)
                {
                    var cellNeighbour = currentCell.Cell.GetNeighbor(HexDirection.SE);
                    if (!closedList.Contains(cellNeighbour) && cellNeighbour.Color == end.Color)
                    {
                        stackIteators.Push(currentCell);

                        currentCell = new HexIterator(cellNeighbour, true, currentCell.Cell);

                        closedList.Add(currentCell.Cell);
                        continue;
                    }
                }
                else if(currentCell.Cell.coordinates.Z < end.coordinates.Z)
                {
                    var cellNeighbour = currentCell.Cell.GetNeighbor(HexDirection.NW);
                    if(!closedList.Contains(cellNeighbour) && cellNeighbour.Color == end.Color)
                    {
                        stackIteators.Push(currentCell);
                        currentCell = new HexIterator(cellNeighbour, true, currentCell.Cell);

                        closedList.Add(currentCell.Cell);
                        continue;
                    }
                }

                currentCell = stackIteators.Pop();

                if(stackIteators.Count == 0)
                    return false;
                    */