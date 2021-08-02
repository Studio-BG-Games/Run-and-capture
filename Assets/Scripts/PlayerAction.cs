using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public float duration = 1f;
    public float distance = 1;

    public ActionType actionType = ActionType.Attack;
    public virtual bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
        {
            return false;
        }
        bool permission = (targetTile != playerState.currentTile);
        return permission;
    }

    public virtual void StartActionOperations(TileInfo targetTile)
    { 
        
    }    



}
