using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagment : MonoBehaviour
{
    public const int BASIC_DIRECTIONS = 6;

    public static List<TileInfo> levelTiles = new List<TileInfo>();

    public static List<List<TileInfo>> charTiles = new List<List<TileInfo>>();

    public static List<PlayerState> players = new List<PlayerState>();

    public static Action OnAnyTileCaptured;

    public List<Material> tileMaterials;    
     
    public static float tileOffset;

    public static Vector3[] basicDirections;

    private void Awake()
    {
        InitCharTiles();
        for (int i = 0; i < transform.childCount; i++)
        {
            var tile = transform.GetChild(i).GetComponent<TileInfo>();
            if (tile)
            {
                levelTiles.Add(tile);
                SetTileStartParams(tile);
                charTiles[(int)tile.tileOwnerIndex].Add(tile);
            }
        }

        tileOffset = Vector3.Distance(levelTiles[0].tilePosition, levelTiles[3].tilePosition);

        basicDirections = GetBasicDirections(BASIC_DIRECTIONS);
        
        //Debug.Log(players.Count);

    }

    private void InitCharTiles()
    {
        for (int i = 0; i < tileMaterials.Count; i++)
        {
            List<TileInfo> charTileList = new List<TileInfo>();
            charTiles.Add(charTileList);  //init empty lists for character tiles
        }
    }

    private void Start()
    {
        //Debug.Log("We have "+ levelTiles.Count + " tiles on this level");
        //Debug.Log("Tiles offset "+ _tilesOffset +" units");
        //Debug.Log(GetTile(new Vector3(0f, 0f, 0f), new Vector3(-0.9f, 0f, 1.7f), 1));
        //pathTiles = Pathfinding.FindPath(levelTiles[0].GetComponent<PathNode>(), levelTiles[106].GetComponent<PathNode>(), tileOffset);
        if (tileMaterials.Count == 0)
        {
            Debug.LogError("You need to set tile materials to TileManagment");
        }


    }

    private void SetTileStartParams(TileInfo tile)
    {
        tile.tilePosition = tile.transform.position;
        tile.GetComponent<Renderer>().material = tileMaterials[(int)tile.tileOwnerIndex];
    }

    public void ChangeTileOwner(TileInfo tile, TileOwner ownerIndex)
    {
        TileOwner oldOwner = tile.tileOwnerIndex;
        tile.tileOwnerIndex = ownerIndex;
        tile.GetComponent<Renderer>().material = tileMaterials[(int)tile.tileOwnerIndex];

        charTiles[(int)ownerIndex].Add(tile);
        charTiles[(int)oldOwner].Remove(tile);
        
        CheckSurroundedTiles(levelTiles, ownerIndex, tile);

        OnAnyTileCaptured?.Invoke();
        
    }

    public static void AssignBuildingToTile(TileInfo tile, GameObject building)
    {
        tile.buildingOnTile = building;
        tile.canMove = false;
        tile.canBuildHere = false;
    }

    public static TileInfo GetTile(Vector3 position)
    {
        TileInfo resultTile = levelTiles[0];
        foreach (TileInfo tile in levelTiles)
        {
            if (Vector3.Distance(position, tile.tilePosition) < Vector3.Distance(position, resultTile.tilePosition))
            {
                resultTile = tile;
            }
        }
        if (Vector3.Distance(position, resultTile.tilePosition) > tileOffset / 2)
        {
            return null;
        }
        else
        {
            return resultTile;
        }
    }

    public static TileInfo GetTile(Vector3 currentTilePosition, Vector3 direction, float distance)
    {
        direction = direction.normalized;
        distance = distance - 0.1f;
        Vector3 tilePos = currentTilePosition + (direction * distance * tileOffset);
        return GetTile(tilePos);
    }

    public static List<TileInfo> GetOtherTiles(TileInfo currentTile, TileOwner ownerIndex)
    {
        List<TileInfo> otherTiles = new List<TileInfo>();
        //int notMyTiles = 0;
        foreach (Vector3 dir in basicDirections)
        {
            var tile = GetTile(currentTile.tilePosition + dir * tileOffset);
            if (tile)
            {
                if (tile.tileOwnerIndex != ownerIndex)
                {
                    //notMyTiles++;
                    otherTiles.Add(tile);
                }
            }
        }
        //Debug.Log("We have " + notMyTiles + " not my tiles around " + currentTile.name);
        return otherTiles;
    }

    public static Vector2 GetDirection(TileInfo currentTile, TileInfo adjacentTile)
    {
        if (!currentTile || !adjacentTile)
            return Vector2.zero;
        Vector3 dir3 = adjacentTile.tilePosition - currentTile.tilePosition;
        Vector2 dir2 = new Vector2(dir3.x, dir3.z);
        return dir2.normalized;
    }

    public static List<TileInfo> GetAllAdjacentTiles(TileInfo currentTile)
    {
        List<TileInfo> allTiles = new List<TileInfo>();        
        foreach (Vector3 dir in basicDirections)
        {
            var tile = GetTile(currentTile.tilePosition + dir * tileOffset);
            if (tile)
            {
                allTiles.Add(tile);
            }
        }        
        return allTiles;
    }

    public static List<TileInfo> GetAllAdjacentTiles(TileInfo currentTile, TileOwner ownerIndex)
    {
        List<TileInfo> allTiles = new List<TileInfo>();
       
        foreach (Vector3 dir in basicDirections)
        {
            var tile = GetTile(currentTile.tilePosition + dir * tileOffset);
            if (tile && ownerIndex == tile.tileOwnerIndex)
            {
                allTiles.Add(tile);
            }
        }        
        return allTiles;
    }

    public static TileInfo GetRandomOtherTile(TileOwner owner)
    {
        int randomIndex = UnityEngine.Random.Range(0, levelTiles.Count - 1);
        while ((levelTiles[randomIndex].tileOwnerIndex == owner) && (levelTiles[randomIndex].canMove == false))
        {
            randomIndex = UnityEngine.Random.Range(0, levelTiles.Count - 1);
        }
        TileInfo otherTile = levelTiles[randomIndex];
        return otherTile;
    }

    public static TileInfo GetNearestOtherTile(TileInfo currentTile, TileOwner owner, float capRadius, Vector3 startPoint)
    {
        var neutralTiles = charTiles[(int)TileOwner.Neutral];
        //Debug.Log("neutral tiles " + neutralTiles.Count);
        TileInfo closestTile = GetRandomOtherTile(owner);
        foreach (TileInfo tile in levelTiles)
        {
            if (tile.canMove && tile!=currentTile && tile.tileOwnerIndex!=owner)
            {
                float distOld = Vector3.Distance(startPoint, closestTile.tilePosition);
               
                float distNew = Vector3.Distance(startPoint, tile.tilePosition);

                float distToTileOld = Vector3.Distance(currentTile.tilePosition, closestTile.tilePosition);
                float distToTileNew = Vector3.Distance(currentTile.tilePosition, tile.tilePosition);

                //Debug.Log("new distance " + distNew);

                if ((distNew <= distOld) && (distNew < capRadius))
                {
                    if ((distToTileNew < distToTileOld))
                    {
                        closestTile = tile;
                    }                                       
                }
            }

        }
        //float dist = Vector3.Distance(startPoint, closestTile.tilePosition);
        //Debug.Log(startPoint);
        //Debug.Log(closestTile);
        //Debug.Log("start point " + startPoint);
        return closestTile;
    }

    public static Vector3[] GetBasicDirections(int directionsAmount)
    {
        Vector3[] tempArr = new Vector3[directionsAmount];
        float deltaAngle = 360 / directionsAmount;
        for (int i = 0; i < directionsAmount; i++)
        {
            tempArr[i] = Quaternion.AngleAxis(i * deltaAngle, Vector3.up) * Vector3.right;
        }
        //Debug.Log(tempArr);
        return tempArr;
    }

    public static void CheckSurroundedTiles(List<TileInfo> tiles, TileOwner ownerIndex, TileInfo capTile)
    {
        //List<TileInfo> firstAdjacentTiles = GetOtherTiles(startTile, ownerIndex);
        //List<TileInfo> firstAllAdjacentTiles = GetAllAdjacentTiles(startTile);
        //List<TileOwner> differentOwners = new List<TileOwner>();
        /*foreach (TileInfo tile in firstAllAdjacentTiles)
        {
            if (!differentOwners.Contains(tile.tileOwnerIndex) && tile.tileOwnerIndex != TileOwner.Neutral)
            {
                differentOwners.Add(tile.tileOwnerIndex);
            }
        }*/
        //Debug.Log(differentOwners.Count);

        /*foreach (var tileOwnerIndex in differentOwners)
        {
            foreach (TileInfo tile in firstAllAdjacentTiles)
            {
                if (tile.tileOwnerIndex != tileOwnerIndex)
                {
                    SetSurroundedTiles(tiles, tileOwnerIndex, tile);
                }
                
            }
        }*/

        /*foreach (TileInfo tile in firstAdjacentTiles)
        {
            SetSurroundedTiles(tiles, ownerIndex, tile);
        }*/

        List<TileOwner> checkingOwners = new List<TileOwner>();
        checkingOwners.Add(capTile.tileOwnerIndex);
        checkingOwners.Add(ownerIndex);

        foreach (TileInfo tile in levelTiles)
        {
            /*tile.checkedFor.Clear();
            tile.easyCaptureFor.Clear();*/
            foreach (TileOwner owner in checkingOwners)
            {
                tile.checkedFor.Remove(owner);
                tile.easyCaptureFor.Remove(owner);
            }            
        }
        foreach (TileInfo tile in levelTiles)
        {
            if (!tile.isBorderTile)
            {                
                /*foreach (var player in players)
                {
                    if ((!tile.checkedFor.Contains(player.ownerIndex)) && (tile.tileOwnerIndex!= player.ownerIndex))
                    {
                        CheckIfSurroundedByOwner(tiles, player.ownerIndex, tile);
                    }
                }*/
                foreach (TileOwner owner in checkingOwners)
                {
                    if ((!tile.checkedFor.Contains(owner)) && (tile.tileOwnerIndex != owner))
                    {
                        CheckIfSurroundedByOwner(tiles, owner, tile);
                    }
                }
            }
            
        }

    }

    public static void CheckIfSurroundedByOwner(List<TileInfo> tiles, TileOwner ownerIndex, TileInfo startTile)
    {
        List<TileInfo> connectedTiles = new List<TileInfo>();
        var q = new Queue<TileInfo>(tiles.Count);
        q.Enqueue(startTile);
        int iterations = 0;

        while (q.Count > 0)
        {
            var tile = q.Dequeue();            
            if (q.Count > tiles.Count)
            {
                throw new Exception("The algorithm is probably looping. Queue size: " + q.Count);
            }

            if (tile.isBorderTile)  //we are in a wrong area
            {                
                connectedTiles.Clear();
                return;
            }

            if (connectedTiles.Contains(tile))
            {
                continue;
            }

            connectedTiles.Add(tile);
            tile.checkedFor.Add(ownerIndex);
            //Debug.Log("Checked");
            var adjacentTiles = GetOtherTiles(tile, ownerIndex);

            foreach (TileInfo newTile in adjacentTiles)
            {
                q.Enqueue(newTile);
            }

            iterations++;
        }       

        foreach (TileInfo tile in connectedTiles)
        {
            tile.easyCaptureFor.Add(ownerIndex);
        }        
    }
   /* public static void SetSurroundedTiles(List<TileInfo> tiles, TileOwner ownerIndex, TileInfo startTile)
    {
        List<TileInfo> surroundedTiles = new List<TileInfo>();
        var q = new Queue<TileInfo>(tiles.Count);
        q.Enqueue(startTile);
        int iterations = 0;

        while (q.Count > 0)
        {
            var tile = q.Dequeue();

            if (q.Count > tiles.Count)
            {
                throw new Exception("The algorithm is probably looping. Queue size: " + q.Count);
            }

            if (tile.isBorderTile)  //we are in a wrong area
            {
                surroundedTiles.Clear();
                return;
            }

            if (surroundedTiles.Contains(tile))
            {
                continue;
            }

            surroundedTiles.Add(tile);

            var adjacentTiles = GetOtherTiles(tile, ownerIndex);
            
            foreach (TileInfo newTile in adjacentTiles)
            {
                q.Enqueue(newTile);
            }

            iterations++;
        }
        //Debug.Log("Found " +surroundedTiles.Count + " tiles");
        //Debug.Log("Iterations " + iterations);

        foreach (TileInfo tile in surroundedTiles)
        {
            tile.whoCanEasyGetTile = ownerIndex;
        }
        Debug.Log("Surrounded " + surroundedTiles.Count);
    }*/
}
