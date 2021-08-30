using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static List<TileInfo> FindPath(TileInfo startTile, TileInfo endTile, List<TileInfo> allTiles, float tileOffset)
    {
        foreach (var tile in allTiles)
        {            
            tile.gCost = 0f;
            tile.hCost = 0f;
            tile.fCost = 0f;
            tile.parent = null;
        }
        var openNodes = new List<TileInfo>();
        var closedNodes = new List<TileInfo>();

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
            foreach (TileInfo newNode in GetAdjacentNodes(currentNode))
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

    private static List<TileInfo> GetAdjacentNodes(TileInfo currentNode)
    {
        var allAjacentTiles = TileManagment.GetAllAdjacentTiles(currentNode);
        List<TileInfo> adjacentNodes = new List<TileInfo>();

        foreach (TileInfo tile in allAjacentTiles)
        {
            if (tile.canMove)
            {
                adjacentNodes.Add(tile);
            }            
        }
        return adjacentNodes;
    }

    private static List<TileInfo> GetPathForNode(TileInfo pathNode)
    {
        var result = new List<TileInfo>();
        var currentNode = pathNode;
        while (currentNode != null)
        {
            result.Add(currentNode);
            currentNode = currentNode.parent;
        }
        result.Reverse();
        //Debug.Log("path found");
        return result;
    }

    private static void SetNodeParams(TileInfo currentNode, TileInfo parrentNode, TileInfo endNode, float nodeOffset)
    {
        currentNode.parent = parrentNode;
        currentNode.gCost = currentNode.parent.gCost + nodeOffset;
        currentNode.hCost = Vector3.Distance(currentNode.transform.position, endNode.transform.position);
        currentNode.fCost = currentNode.gCost + currentNode.hCost;
    }
}
