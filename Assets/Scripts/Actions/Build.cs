using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Build", menuName = "Actions/New Build")]
public class Build : PlayerAction
{
    public GameObject buildPref;
    public override bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
            return false;
        bool permission = base.IsActionAllowed(targetTile, playerState);
        bool isMyTile = targetTile.tileOwnerIndex == playerState.ownerIndex;
        permission = permission && targetTile.canBuildHere && isMyTile;
        return permission;
    }

    public override void StartActionOperations(TileInfo targetTile, PlayerState currentPlayer)
    {
        base.StartActionOperations(targetTile, currentPlayer);
        var spawnedTower = Instantiate(buildPref, targetTile.tilePosition, buildPref.transform.rotation);
        int activeModelIndex = (int)currentPlayer.ownerIndex - 1;
        spawnedTower.transform.GetChild(activeModelIndex).gameObject.SetActive(true);
        TileManagment.AssignBuildingToTile(targetTile, spawnedTower);
    }
}
