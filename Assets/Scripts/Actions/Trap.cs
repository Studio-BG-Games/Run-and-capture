using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trap", menuName = "Actions/New Trap")]
public class Trap : PlayerAction
{
    public GameObject trapPref;

    private TileInfo _target;
    public override bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
            return false;
        bool permission = base.IsActionAllowed(targetTile, playerState);
        bool isMyTile = targetTile.tileOwnerIndex == playerState.ownerIndex;
        permission = permission && targetTile.canBuildHere && isMyTile;

        bool ifCanPlaceAnotherTrap = CheckExit(playerState);
        if (!ifCanPlaceAnotherTrap)
        {
            return false;
        }
        return permission;
    }

    private bool CheckExit(PlayerState playerState)
    {
        bool permision = false;
        int possibleMoveTiles = 0;
        List<TileInfo> adjacentTiles = TileManagment.GetAllAdjacentTiles(playerState.currentTile);
        foreach (var tile in adjacentTiles)
        {
            if (tile.canMove && tile.buildingOnTile == null)
            {
                possibleMoveTiles++;
            }
        }
        //Debug.Log(possibleMoveTiles);
        if (possibleMoveTiles > 1)
        {
            return true;
        }
        return permision;
    }

    public override void StartActionOperations(TileInfo targetTile, PlayerState currentPlayer)
    {
        base.StartActionOperations(targetTile, currentPlayer);
        _target = targetTile;
        _target.canBuildHere = false;
    }

    public override void FinishActionOperations(PlayerState currentPlayer)
    {
        base.FinishActionOperations(currentPlayer);
        var spawnedTrap = Instantiate(trapPref, _target.tilePosition, trapPref.transform.rotation);
        spawnedTrap.GetComponent<TrapObj>().SetOwner(currentPlayer.ownerIndex);
        TileManagment.AssignBuildingToTile(_target, spawnedTrap);
        _target.canMove = true;
    }
}
