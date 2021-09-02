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
        return permission;
    }

    public override void StartActionOperations(TileInfo targetTile)
    {
        base.StartActionOperations(targetTile);
        _target = targetTile;
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
