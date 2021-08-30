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

    public override void StartActionOperations(TileInfo targetTile)
    {
        base.StartActionOperations(targetTile);
        var spawnedTower = Instantiate(buildPref, targetTile.tilePosition, buildPref.transform.rotation);
        TileManagment.AssignBuildingToTile(targetTile, spawnedTower);
        //Debug.Log("startBuilding");
    }
}
