using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "New SuperJump", menuName = "Actions/New SuperJump")]
public class SuperJump : PlayerAction
{
    //public GameObject actionPref;
    //public GameObject standartAttackGroundImpact;

    private TileInfo _target;

    private List<TileInfo> _capTiles = new List<TileInfo>();

    public override bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
            return false;
        bool permission = base.IsActionAllowed(targetTile, playerState);
        permission = permission && targetTile.canBeAttacked;
        _capTiles = GetActualCapTargets(GameData.playerLevel, targetTile, playerState);
        foreach (var enemy in playerState.enemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }
            if (_capTiles.Contains(enemy.currentTile))
            {
                return false;
            }
        }
        return permission;
    }

    public override void FinishActionOperations(PlayerState currentPlayer)
    {
        base.FinishActionOperations(currentPlayer);
        currentPlayer.currentTile = _target;
        _target = null;
    }

    public override void Impact(TileInfo targetTile, PlayerState currentPlayer)
    {
        base.Impact(targetTile, currentPlayer);        

        var capController = currentPlayer.GetComponent<CaptureController>();

        foreach (var enemy in currentPlayer.enemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }
            if (_capTiles.Contains(enemy.currentTile))
            {
                return;
            }
        }

        foreach (TileInfo tile in _capTiles)
        {
            if (tile == null)
            {
                continue;
            }
            if (tile.tileOwnerIndex != currentPlayer.ownerIndex)
            {
                capController.CaptureTile(tile);
                if (tile.buildingOnTile != null)
                {
                    Destroy(tile.buildingOnTile);
                    TileManagment.ReleaseTile(tile);
                }
            }
        }
        
    }

    public override void StartActionOperations(TileInfo targetTile, PlayerState currentPlayer)
    {
        base.StartActionOperations(targetTile, currentPlayer);
        _target = targetTile;
        currentPlayer.transform.DOMove(_target.tilePosition, duration);
        currentPlayer.transform.LookAt(_target.tilePosition);

        currentPlayer.targetMoveTile = _target;
        currentPlayer.targetMoveTile.canMove = false;
        currentPlayer.currentTile.canMove = true;

        //moveVFX.Play();
    }

    private List<TileInfo> GetAllPossibleCapTargets(TileInfo targetTile, PlayerState playerState)
    {
        Vector3 playerDir = targetTile.tilePosition - playerState.currentTile.tilePosition;
        Vector3 playerDirR = Quaternion.AngleAxis(-60, Vector3.up) * playerDir;
        Vector3 playerDirL = Quaternion.AngleAxis(60, Vector3.up) * playerDir;
        List<TileInfo> tiles = new List<TileInfo>();
        tiles.Add(targetTile); //0
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + playerDir)); //1
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 2*playerDir)); //2
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 3*playerDir)); //3        
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 2*playerDir+ playerDirL)); //4     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 2 * playerDir + playerDirR)); //5     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + playerDir + playerDirL)); //6     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + playerDir + playerDirR)); //7     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 4 * playerDir)); //8     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 3*playerDir + playerDirL)); //9     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 3 * playerDir + playerDirR)); //10     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 2 * playerDir + 2 * playerDirL)); //11    
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 2 * playerDir + 2 * playerDirR)); //12     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + playerDir + 2 * playerDirL)); //13    
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + playerDir + 2 * playerDirR)); //14     
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 2 * playerDirL)); //15    
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + 2 * playerDirR)); //16
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + playerDirL)); //17    
        tiles.Add(TileManagment.GetTile(tiles[0].tilePosition + playerDirR)); //18     

        return tiles;
    }

    private List<TileInfo> GetActualCapTargets(int playerLevel, TileInfo target, PlayerState player)
    {
        List<TileInfo> allTargets = GetAllPossibleCapTargets(target, player);
        List<TileInfo> actualTargets = new List<TileInfo>();
        for (int i = 0; i <= playerLevel; i++)
        {
            actualTargets.Add(allTargets[i]);
        }

        return actualTargets;
    }
}
