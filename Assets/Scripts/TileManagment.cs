using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagment : MonoBehaviour
{
    public const int BASIC_DIRECTIONS = 6;

    public static List<TileInfo> levelTiles = new List<TileInfo>();

    public static List<List<TileInfo>> charTiles = new List<List<TileInfo>>();

    public static Action OnInitialized;
    public static Action<PlayerState> OnAnyTileCaptured;

    public static List<Material> tileMaterialsStatic;

    public static float tileOffset;

    public static Vector3[] basicDirections;

    [SerializeField]
    private List<Material> _tileMaterials;

    [SerializeField]
    private Transform _tileParent;

    private void Awake()
    {
        InitTileManager();        
    }

    private void InitTileManager()
    {
        SetStaticTileMaterials();
        InitCharacterTiles();
        for (int i = 0; i < _tileParent.childCount; i++)
        {
            var tile = _tileParent.GetChild(i).GetComponent<TileInfo>();
            if (tile)
            {
                levelTiles.Add(tile);
                SetTileStartParams(tile);
                charTiles[(int)tile.tileOwnerIndex].Add(tile);
            }
        }

        basicDirections = GetBasicDirections(BASIC_DIRECTIONS);
        tileOffset = GetTileOffset(levelTiles);

        //Debug.Log("tile offset is "+ tileOffset);
        OnInitialized?.Invoke();
        
    }

    private float GetTileOffset(List<TileInfo> tiles)
    {
        TileInfo firstTile = tiles[0];
        TileInfo secondTile = tiles[1];

        return Vector3.Distance(firstTile.tilePosition, secondTile.tilePosition);
    }
    private void SetStaticTileMaterials()
    {
        tileMaterialsStatic = new List<Material>();
        foreach (var mat in _tileMaterials)
        {
            tileMaterialsStatic.Add(mat);
        }
    }
    private void InitCharacterTiles()
    {
        for (int i = 0; i < tileMaterialsStatic.Count; i++)
        {
            List<TileInfo> charTileList = new List<TileInfo>();
            charTiles.Add(charTileList);  //init empty lists for character tiles
        }
    }
    private void SetTileStartParams(TileInfo tile)
    {
        tile.tilePosition = tile.transform.position;
        tile.GetComponent<Renderer>().material = tileMaterialsStatic[(int)tile.tileOwnerIndex];
    }

    public static void ChangeTileOwner(TileInfo tile, PlayerState newPlayer)
    {
        TileOwner newOwner = newPlayer.ownerIndex;
        TileOwner oldOwner = tile.tileOwnerIndex;
        tile.tileOwnerIndex = newOwner;
        tile.GetComponent<Renderer>().material = tileMaterialsStatic[(int)tile.tileOwnerIndex];

        charTiles[(int)newOwner].Add(tile);
        charTiles[(int)oldOwner].Remove(tile);

        SetTilesCapState(newOwner, oldOwner, levelTiles, GameManager.players);
        OnAnyTileCaptured?.Invoke(newPlayer);

    }

    public static void SetTilesCapState(TileOwner ownerNew, TileOwner owerOld, List<TileInfo> allTiles, List<PlayerState> players)
    {
        CheckSurroundedTiles(allTiles, ownerNew, owerOld);
        //SetAllCharTilesStates(players);
    }

    public static void AssignBuildingToTile(TileInfo tile, GameObject building)
    {
        tile.buildingOnTile = building;
        tile.canMove = false;
        tile.canBuildHere = false;
    }

    public static void ReleaseTile(TileInfo tile)
    {
        tile.buildingOnTile = null;
        tile.canMove = true;
        tile.canBuildHere = true;
    }

    public static void SetTileAvailable(TileInfo tile)
    {
        tile.canMove = true;
        tile.canBuildHere = true;
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

    public static TileInfo GetTile(TileOwner owner)
    {
        var ownerTiles = charTiles[(int)owner];
        int randomTileIndex = UnityEngine.Random.Range(0, ownerTiles.Count - 1);

        TileInfo resultTile = ownerTiles[randomTileIndex];
        while (!resultTile.canMove)
        {
            randomTileIndex = UnityEngine.Random.Range(0, ownerTiles.Count - 1);
            resultTile = ownerTiles[randomTileIndex];
        }

        return resultTile;
    }

    public static TileInfo GetRandomOtherTile(TileOwner owner)
    {
        int randomTargetOwner = UnityEngine.Random.Range(0, charTiles.Count);
        var searchingTiles = charTiles[randomTargetOwner];
        while (searchingTiles.Count == 0  ||  randomTargetOwner == (int)owner)
        {
            randomTargetOwner = UnityEngine.Random.Range(0, charTiles.Count);
            searchingTiles = charTiles[randomTargetOwner];
        }
        int randomTileIndex = UnityEngine.Random.Range(0, searchingTiles.Count);
        TileInfo otherTile = searchingTiles[randomTileIndex];
        if (!otherTile.canMove)
        {
            return GetRandomOtherTile(owner);
        }
        else
        {
            return otherTile;
        }        
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
        
        return otherTiles;
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

    public static List<TileInfo> GetOwnerAdjacentTiles(TileInfo currentTile, TileOwner ownerIndex)
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

    public static List<TileInfo> GetCharacterTiles(PlayerState character)
    {
        TileOwner owner = character.ownerIndex;
        List<TileInfo> playerTiles = new List<TileInfo>();        
        foreach (TileInfo tile in charTiles[(int)owner])
        {
            playerTiles.Add(tile);
        }        
        return playerTiles;
    }

    public static void SetLockState(List<TileInfo> tiles)
    {        
        foreach (TileInfo tile in tiles)
        {
            tile.isLocked = true;
            tile.easyCapForAll = true;
        }
        Debug.Log("easycapForAll");
    }

    public static void SetEasyCaptureForAll(PlayerState player)
    {
        List<TileInfo> tiles = charTiles[(int)player.ownerIndex];
        foreach (TileInfo tile in tiles)
        {
            tile.isLocked = true;
            tile.easyCapForAll = true;
        }        
    }

    public static void RemoveLockState(List<TileInfo> tiles)
    {
        foreach (TileInfo tile in tiles)
        {            
            tile.isLocked = false;
            tile.easyCapForAll = false;
        }
    }       

    public static TileInfo GetClosestOtherTile(TileInfo currentTile, TileOwner owner,/* float capRadius,*/ Vector3 startPoint)
    {
        var neutralTiles = charTiles[(int)TileOwner.Neutral];
        //Debug.Log("neutral tiles " + neutralTiles.Count);
        TileInfo closestTile = neutralTiles[0];
        foreach (TileInfo tile in levelTiles)
        {
            if (tile.canMove && tile != currentTile && tile.tileOwnerIndex != owner)
            {
                float distOld = Vector3.Distance(startPoint, closestTile.tilePosition);

                float distNew = Vector3.Distance(startPoint, tile.tilePosition);

                float playerDistOld = Vector3.Distance(currentTile.tilePosition, closestTile.tilePosition);
                float playerDistNew = Vector3.Distance(currentTile.tilePosition, tile.tilePosition);

                //Debug.Log("new distance " + distNew);

                if ((distNew <= distOld) /*&& (distNew < capRadius)*/)
                {
                    if ((playerDistNew < playerDistOld))
                    {
                        closestTile = tile;
                    }
                }
            }

        }
        
        return closestTile;
    }

    public static void CheckSurroundedTiles(List<TileInfo> tiles, TileOwner newOwner, TileOwner oldOwner)
    {
        List<TileOwner> checkPlayers = new List<TileOwner>();
        if (oldOwner != TileOwner.Neutral)
        {
            checkPlayers.Add(oldOwner);
        }        
        checkPlayers.Add(newOwner);

        foreach (TileInfo tile in levelTiles)
        {
            tile.checkedFor.Clear();
            foreach (var owner in checkPlayers)
            {                
                //tile.checkedFor.Remove(newOwner);
                if (!tile.isLocked)
                {
                    tile.easyCaptureFor.Remove(owner);
                    //tile.easyCaptureFor.Clear();
                }
            }

            
        }

        foreach (var player in checkPlayers)
        {
            foreach (TileInfo tile in levelTiles)
            {
                if (!tile.isBorderTile)
                {
                    if ((!tile.checkedFor.Contains(player)) && (tile.tileOwnerIndex != player))
                    {
                        CheckIfSurroundedByOwner(tiles, player, tile);
                    }
                }

            }
        }

    }

    public static void SetCharTilesState(PlayerState player)
    {
        //Debug.Log("set chartiles state");
        List<TileInfo> allPlayerTiles = charTiles[(int)player.ownerIndex];
        foreach (TileInfo tile in allPlayerTiles)
        {
            tile.easyCapForAll = true;
        }
        List<TileInfo> playerConnectedTiles = GetConnectedTiles(levelTiles, player.ownerIndex, player.currentTile);
        foreach (TileInfo tile in playerConnectedTiles)
        {
            if (!tile.isLocked)
            {
                tile.easyCapForAll = false;
            }            
        }
    }

    public static void SetAllCharTilesStates(List<PlayerState> players)
    {
        foreach (var player in players)
        {
            SetCharTilesState(player);
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
            if (!tile.isLocked)
            {
                tile.easyCaptureFor.Add(ownerIndex);
            }
        }
    }

    public static List<TileInfo> GetConnectedTiles(List<TileInfo> allTiles, TileOwner ownerIndex, TileInfo startTile)
    {
        List<TileInfo> connectedTiles = new List<TileInfo>();
        var q = new Queue<TileInfo>(allTiles.Count);
        q.Enqueue(startTile);
        int iterations = 0;

        while (q.Count > 0)
        {
            var tile = q.Dequeue();
            if (q.Count > allTiles.Count)
            {
                throw new Exception("The algorithm is probably looping. Queue size: " + q.Count);
            }          

            if (connectedTiles.Contains(tile))
            {
                continue;
            }

            connectedTiles.Add(tile);
            
            var adjacentTiles = GetOwnerAdjacentTiles(tile, tile.tileOwnerIndex);

            foreach (TileInfo newTile in adjacentTiles)
            {
                q.Enqueue(newTile);
            }
            iterations++;
        }
        //Debug.Log("there are " + connectedTiles.Count + " near the " + ownerIndex);
        return connectedTiles;
    }

    /*public static void CheckIfSurroundedByOwner(List<TileInfo> tiles, TileOwner ownerIndex, TileInfo startTile)
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
            if (!tile.isLocked)
            {
                tile.easyCaptureFor.Add(ownerIndex);
            }
        }
    }*/

    private Vector3[] GetBasicDirections(int directionsAmount)
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
    
}
