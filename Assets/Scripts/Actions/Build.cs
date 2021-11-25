using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Build", menuName = "Actions/New Build")]
public class Build : PlayerAction
{
    public GameObject buildPref;
    public GameObject prefVFX;
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
        targetTile.canMove = false;
        var spawnedTower = Instantiate(buildPref, targetTile.tilePosition, buildPref.transform.rotation);
        spawnedTower.GetComponent<ToweHealthController>().owner = currentPlayer.ownerIndex;
        //SetEffect();
        int activeModelIndex = (int)currentPlayer.ownerIndex - 1;
        spawnedTower.transform.GetChild(activeModelIndex).gameObject.SetActive(true);
        TileManagment.AssignBuildingToTile(targetTile, spawnedTower);
    }

    private void SetEffect()
    {
        if(prefVFX != null)
        {
            var effect = Instantiate(prefVFX, buildPref.transform.position, buildPref.transform.rotation);
        }
        else
        {
            prefVFX = null;
        }
    }
}
