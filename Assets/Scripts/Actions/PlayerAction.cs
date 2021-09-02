using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : ScriptableObject
{
    public float duration = 1f;
    public float distance = 1;

    public CharacterState actionState = CharacterState.Attack;

    //public CharacterSubState actionType = CharacterSubState.Attack;
    public virtual bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
        {
            return false;
        }
       
        return targetTile != playerState.currentTile;
    }

    public virtual void StartActionOperations(TileInfo targetTile)
    {

    }

    public virtual void Impact(TileInfo targetTile, TileInfo currentTile, TileOwner owner)
    {

    }

    public virtual void FinishActionOperations(PlayerState currentPlayer)
    {
        Debug.Log("final action");
    }
}

public enum ActionType
{
    Attack,
    Build,
    Capture,
    Null
}
