using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : ScriptableObject
{
    public float maxAttackEnergy = 3f;
    public float attackResetTime = 3f;
    public float attackCost = 1f;
    public float duration = 1f;
    public float distance = 1;

    //public CharacterState actionState = CharacterState.Attack;

    public ActionType actionType = ActionType.Attack;
    public virtual bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
        {
            return false;
        }
       
        return targetTile != playerState.currentTile;
    }

    public virtual void StartActionOperations(TileInfo targetTile, PlayerState currentPlayer)
    {

    }

    public virtual void Impact(TileInfo targetTile, PlayerState currentPlayer)
    {

    }

    public virtual void FinishActionOperations(PlayerState currentPlayer)
    {
        //Debug.Log("final action");
    }
}
