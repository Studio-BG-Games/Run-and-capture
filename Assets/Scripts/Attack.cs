using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : PlayerAction
{
    public override bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
            return false;
        bool permission = base.IsActionAllowed(targetTile, playerState);
        permission = permission && targetTile.canBeAttacked;
        return permission;
    }
}
