using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tree Attack", menuName = "Actions/New Tree Attack")]
public class TreeAttack : PlayerAction
{
    public GameObject attackPref;
    //public GameObject standartAttackGroundImpact;

    public float damage = 100f;

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
        var currentProjectile = Instantiate(attackPref, currentTile.tilePosition + attackPref.transform.position, attackPref.transform.rotation);
        currentProjectile.transform.LookAt(targetTile.tilePosition + attackPref.transform.position);
        //InitAttack(currentTile.tilePosition, direction, owner);
        TreeHealthController tree = targetTile.buildingOnTile.GetComponent<TreeHealthController>();
        if (tree != null)
        {
            tree.TakeDamage(damage);
        }
    }

    private void InitAttack(Vector3 startPosition, Vector3 direction, TileOwner projOwner)
    {
        //var currentProjectile = Instantiate(standartAttackPref, startPosition, standartAttackPref.transform.rotation).GetComponent<ProjectileController>();
        //currentProjectile.SetinitialParams(projOwner, direction, TileManagment.tileOffset);

        //Instantiate(standartAttackGroundImpact, startPosition, standartAttackGroundImpact.transform.rotation);
    }
}
