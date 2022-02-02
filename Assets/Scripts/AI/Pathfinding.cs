using System;
using System.Collections.Generic;
using HexFiled;

namespace AI
{
    public static class Pathfinding
    {
        public static void FindPath(HexCell startTile, HexCell endTile, Queue<HexDirection> path)
        {
            var foundPathIter = 0;
            var currentTile = startTile;
            var previoustile = currentTile;

            if (startTile == null || endTile == null)
            {
                return;
            }

            while (foundPathIter < 3 && currentTile != null)
            {
                HexDirection dir = HexDirection.E;
                if (currentTile.coordinates.Z == endTile.coordinates.Z)
                {
                    if (currentTile.coordinates.X < endTile.coordinates.X)
                    {
                        dir = HexDirection.E;
                    }

                    else if (currentTile.coordinates.X > endTile.coordinates.X)
                    {
                        dir = HexDirection.W;
                    }
                }

                else if (currentTile.coordinates.Y == endTile.coordinates.Y)
                {
                    if (currentTile.coordinates.X > endTile.coordinates.X)
                    {
                        dir = HexDirection.NW;
                    }
                    else if (currentTile.coordinates.X < endTile.coordinates.X)
                    {
                        dir = HexDirection.SE;
                    }
                }

                else if (currentTile.coordinates.X == endTile.coordinates.X)
                {
                    if (currentTile.coordinates.Y > endTile.coordinates.Y)
                    {
                        dir = HexDirection.NE;
                    }
                    else if (currentTile.coordinates.Y < endTile.coordinates.Y)
                    {
                        dir = HexDirection.SW;
                    }
                }

                else
                {
                    if (Math.Abs(currentTile.coordinates.X - endTile.coordinates.X) <
                        Math.Abs(currentTile.coordinates.Y - endTile.coordinates.Y))
                    {
                        if (Math.Abs(currentTile.coordinates.X - endTile.coordinates.X) <
                            Math.Abs(currentTile.coordinates.Z - endTile.coordinates.Z))
                        {
                            dir = currentTile.coordinates.X > endTile.coordinates.X
                                ? HexDirection.E
                                : HexDirection.W;
                           
                        }
                        else
                        {
                            dir = currentTile.coordinates.Z > endTile.coordinates.Z
                                ? HexDirection.SW
                                : HexDirection.NE;
                            
                        }
                    }
                    else
                    {
                        if (Math.Abs(currentTile.coordinates.Y - endTile.coordinates.Y) <
                            Math.Abs(currentTile.coordinates.Z - endTile.coordinates.Z))
                        {
                            dir = currentTile.coordinates.Y > endTile.coordinates.Y
                                ? HexDirection.SE
                                : HexDirection.NW;
                            
                        }
                        else
                        {
                            dir = currentTile.coordinates.Z > endTile.coordinates.Z
                                ? HexDirection.SW
                                : HexDirection.NE;
                            
                        }
                    }
                }

                if (currentTile == endTile)
                {
                    return;
                }




                while (currentTile.GetNeighbor(dir) == null)
                {
                    dir = dir.PlusSixtyDeg();
                }

                currentTile = currentTile.GetNeighbor(dir);
                path.Enqueue(dir);
                foundPathIter++;
            }
        }
    }
}