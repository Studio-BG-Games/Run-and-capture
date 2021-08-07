using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static List<TileInfo> FindPath(TileInfo startTile, TileInfo endTile, float tileOffset)
    {
        var openNodes = new List<PathNode>();
        var closedNodes = new List<PathNode>();

        var startNode = startTile.GetComponent<PathNode>();
        var endNode = endTile.GetComponent<PathNode>();
        startNode.gCost = 0f;
        startNode.hCost = Vector3.Distance(startNode.transform.position, endNode.transform.position);
        startNode.fCost = startNode.gCost + startNode.hCost;
        openNodes.Add(startNode);

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

            if (currentNode == endNode)
            {
                return GetPathForNode(currentNode);
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
            Debug.Log(currentNode.name);
            foreach (PathNode newNode in GetAdjacentNodes(currentNode))
            {
                if (newNode)
                {
                    if (closedNodes.Contains(newNode))
                    {
                        continue;                        
                    }                    
                    if (!openNodes.Contains(newNode))
                    {
                        SetNodeParams(newNode, currentNode, endNode, tileOffset);
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

    private static List<PathNode> GetAdjacentNodes(PathNode currentNode)
    {
        var allAjacentTiles = TileManagment.GetAllTiles(currentNode.GetComponent<TileInfo>());
        List<PathNode> adjacentNodes = new List<PathNode>();

        foreach (TileInfo tile in allAjacentTiles)
        {
            if (tile.canMove)
            {
                adjacentNodes.Add(tile.GetComponent<PathNode>());
            }            
        }
        return adjacentNodes;
    }

    private static List<TileInfo> GetPathForNode(PathNode pathNode)
    {
        var result = new List<TileInfo>();
        var currentNode = pathNode;
        while (currentNode != null)
        {
            result.Add(currentNode.GetComponent<TileInfo>());
            currentNode = currentNode.parent;
        }
        result.Reverse();
        Debug.Log("path found");
        return result;
    }

    private static void SetNodeParams(PathNode currentNode, PathNode parrentNode, PathNode endNode, float nodeOffset)
    {
        currentNode.parent = parrentNode;
        currentNode.gCost = currentNode.parent.gCost + nodeOffset;
        currentNode.hCost = Vector3.Distance(currentNode.transform.position, endNode.transform.position);
        currentNode.fCost = currentNode.gCost + currentNode.hCost;
    }
}
