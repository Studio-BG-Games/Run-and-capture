using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Actions/New Attack")]
public class Attack : PlayerAction
{
    public GameObject standartAttackPref;
    public GameObject standartAttackGroundImpact;
    
    public override bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
            return false;
        bool permission = base.IsActionAllowed(targetTile, playerState);
        permission = permission && targetTile.canBeAttacked;        
        return permission;
    }

    public override void Impact(TileInfo targetTile, TileInfo currentTile, TileOwner owner)
    {
        base.Impact(targetTile, currentTile, owner);
        Vector3 direction = targetTile.tilePosition - currentTile.tilePosition;
        InitAttack(currentTile.tilePosition, direction, owner);
    }

    private void InitAttack(Vector3 startPosition, Vector3 direction, TileOwner projOwner)
    {
        var currentProjectile = Instantiate(standartAttackPref, startPosition, standartAttackPref.transform.rotation).GetComponent<ProjectileController>();
        currentProjectile.SetinitialParams(projOwner, direction, TileManagment.tileOffset);

        Instantiate(standartAttackGroundImpact, startPosition, standartAttackGroundImpact.transform.rotation);
    }    
}
