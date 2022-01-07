using System.Collections.Generic;
using System.Linq;
using HexFiled;
using UnityEngine;

namespace AI
{
    public static class Pathfinding
    {
        public static Queue<HexCell> FindPath(HexCell startTile, HexCell endTile, List<HexCell> allTiles)
        {
            foreach (var tile in allTiles)
            {            
                tile.gCost = 0f;
                tile.hCost = 0f;
                tile.fCost = 0f;
            
            }

            var tileOffset = HexGrid.HexDistance;
            var openNodes = new List<HexCell>();
            var closedNodes = new List<HexCell>();

            startTile.gCost = 0f;
            startTile.hCost = Vector3.Distance(startTile.transform.position, endTile.transform.position);
            startTile.fCost = startTile.gCost + startTile.hCost;
            startTile.parent = null;
            openNodes.Add(startTile);

            while (openNodes.Count > 0)
            {
                var currentNode = openNodes[0]; //looking for lowest value node
                foreach (var node in openNodes)
                {
                    if (node.fCost < currentNode.fCost)
                    {
                        currentNode = node;
                    }
                }

                if (currentNode == endTile)
                {
                    return GetPathForNode(currentNode);
                }

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);
                //Debug.Log(currentNode.name);
                foreach (HexCell newNode in currentNode.GetListNeighbours())
                {
                    if (newNode)
                    {
                        if (closedNodes.Contains(newNode))
                        {
                            continue;                        
                        }                    
                        if (!openNodes.Contains(newNode))
                        {
                            SetNodeParams(newNode, currentNode, endTile, tileOffset);
                            openNodes.Add(newNode);
                        }
                        else 
                        {
                            if (currentNode.gCost + tileOffset < newNode.gCost)
                            {
                                newNode.gCost = currentNode.gCost + tileOffset;
                                newNode.parent = currentNode;
                                newNode.fCost = newNode.gCost + newNode.hCost;
                            }
                        }
                    }
                }
            }
            Debug.Log("path not found");
            return null;
        }

        // private static List<TileInfo> GetAdjacentNodes(TileInfo currentNode)
        // {
        //     var allAjacentTiles = currentNode.Cell.GetListNeighbours();
        //     List<TileInfo> adjacentNodes = new List<TileInfo>();
        //
        //     foreach (TileInfo tile in allAjacentTiles)
        //     {
        //         if (tile.canMove)
        //         {
        //             adjacentNodes.Add(tile);
        //         }            
        //     }
        //     return adjacentNodes;
        // }

        private static Queue<HexCell> GetPathForNode(HexCell pathNode)
        {
            var result = new Queue<HexCell>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Enqueue(currentNode);
                currentNode = currentNode.parent;
            }
            
            return result;
        }

        private static void SetNodeParams(HexCell currentNode, HexCell parrentNode, HexCell endNode, float nodeOffset)
        {
            currentNode.parent = parrentNode;
            currentNode.gCost = currentNode.parent.gCost + nodeOffset;
            currentNode.hCost = Vector3.Distance(currentNode.transform.position, endNode.transform.position);
            currentNode.fCost = currentNode.gCost + currentNode.hCost;
        }
    }
}
