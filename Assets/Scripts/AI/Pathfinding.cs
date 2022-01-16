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

            if (startTile == null || endTile == null)
            {
                return;
            }
            while (foundPathIter < 3 && currentTile == null)
            {
                
                if (currentTile.coordinates.Z == endTile.coordinates.Z)
                {
                    if (currentTile.coordinates.X < endTile.coordinates.X)
                    {
                        path.Enqueue(HexDirection.E);
                        currentTile = currentTile.GetNeighbor(HexDirection.E);
                    }

                    else if (currentTile.coordinates.X > endTile.coordinates.X)
                    {
                        path.Enqueue(HexDirection.W);
                        currentTile = currentTile.GetNeighbor(HexDirection.W);
                    }
                }

                else if (currentTile.coordinates.Y == endTile.coordinates.Y)
                {
                    if (currentTile.coordinates.X > endTile.coordinates.X)
                    {
                        path.Enqueue(HexDirection.NW);
                        currentTile = currentTile.GetNeighbor(HexDirection.NW);
                    }
                    else if (currentTile.coordinates.X < endTile.coordinates.X)
                    {
                        path.Enqueue(HexDirection.SE);
                        currentTile = currentTile.GetNeighbor(HexDirection.SE);
                    }
                }

                else if (currentTile.coordinates.X == endTile.coordinates.X)
                {
                    if (currentTile.coordinates.Y > endTile.coordinates.Y)
                    {
                        path.Enqueue(HexDirection.NE);
                        currentTile = currentTile.GetNeighbor(HexDirection.NE);
                    }
                    else if (currentTile.coordinates.Y < endTile.coordinates.Y)
                    {
                        path.Enqueue(HexDirection.SW);
                        currentTile = currentTile.GetNeighbor(HexDirection.SW);
                    }
                }

                else
                {
                    if (Math.Abs(currentTile.coordinates.X - endTile.coordinates.X) < Math.Abs(currentTile.coordinates.Y - endTile.coordinates.Y))
                    {
                        if (Math.Abs(currentTile.coordinates.X - endTile.coordinates.X) <
                            Math.Abs(currentTile.coordinates.Z - endTile.coordinates.Z))
                        {
                            var dir = currentTile.coordinates.X > endTile.coordinates.X
                                ? HexDirection.E
                                : HexDirection.W;
                            path.Enqueue(dir);
                            currentTile = currentTile.GetNeighbor(dir);
                        }
                        else
                        {
                            var dir = currentTile.coordinates.Z > endTile.coordinates.Z
                                ? HexDirection.SW
                                : HexDirection.NE;
                            path.Enqueue(dir);
                            currentTile = currentTile.GetNeighbor(dir);
                        }
                    }
                    else
                    {
                        if (Math.Abs(currentTile.coordinates.Y - endTile.coordinates.Y) <
                            Math.Abs(currentTile.coordinates.Z - endTile.coordinates.Z))
                        {
                            var dir = currentTile.coordinates.Y > endTile.coordinates.Y
                                ? HexDirection.SE
                                : HexDirection.NW;
                            path.Enqueue(dir);
                            currentTile = currentTile.GetNeighbor(dir);
                        }
                        else
                        {
                            var dir = currentTile.coordinates.Z > endTile.coordinates.Z
                                ? HexDirection.SW
                                : HexDirection.NE;
                            path.Enqueue(dir);
                            currentTile = currentTile.GetNeighbor(dir);
                        }
                    }

                }
                if (currentTile == endTile)
                {
                    return;
                }

                foundPathIter++;
            }
        }
        
    }
}
