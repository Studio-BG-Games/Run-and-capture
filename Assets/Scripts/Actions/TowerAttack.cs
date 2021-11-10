using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New TowerAttack", menuName = "Actions/New TowerAttack")]
public class TowerAttack : PlayerAction
{

    public GameObject standartAttackPref;
    public GameObject standartAttackGroundImpact;

    public AudioClip throw_SFX;

    //
    //[SerializeField] private Transform _offset;
    
    
    public override bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
            return false;
        bool permission = base.IsActionAllowed(targetTile, playerState);
        //bool isMyTile = targetTile.tileOwnerIndex == playerState.ownerIndex; && isMyTile
        permission = permission && targetTile.canBeAttacked;        
        return permission;
    }

    public override void StartActionOperations(TileInfo targetTile, PlayerState currentPlayer)
    {
        base.StartActionOperations(targetTile, currentPlayer);
        
        //currentPlayer.GetComponent<AudioController>().PlaySound(throw_SFX);

    }

    public override void Impact(TileInfo targetTile, PlayerState currentPlayer)
    {
        base.Impact(targetTile, currentPlayer);
        Vector3 direction = (targetTile.tilePosition  ) - currentPlayer.currentTile.tilePosition ;
        InitAttack(currentPlayer.currentTile.tilePosition, direction  , currentPlayer.ownerIndex);
        //currentPlayer.GetComponent<AudioController>().PlaySound(throw_SFX);
    }

    private void InitAttack(Vector3 startPosition, Vector3 direction, TileOwner projOwner)
    {
        var currentProjectile = Instantiate(standartAttackPref, startPosition, standartAttackPref.transform.rotation).GetComponent<ProjectileController>();
        currentProjectile.SetinitialParams(projOwner, direction, TileManagment.tileOffset);
        //
        currentProjectile.GetComponent<ProjectileController>().owner = projOwner;
        int activeModelIndex = (int)projOwner - 1;
        currentProjectile.transform.GetChild(activeModelIndex).gameObject.SetActive(true);
        //

        Instantiate(standartAttackGroundImpact, startPosition, standartAttackGroundImpact.transform.rotation);
    }    
}
