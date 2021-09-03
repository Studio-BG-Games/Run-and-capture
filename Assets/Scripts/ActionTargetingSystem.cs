using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionTargetingSystem : MonoBehaviour
{
    
    public Action<TileInfo, ActionType> OnFoundTarget;
    public Action OnLostTarget;

    private PlayerState _playerState;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();        

        if (_playerState.controlType == ControlType.Player)
        {
            CustomInput.OnTouchDown += StartTargeting;            
        }

        //OnFoundTarget += WriteSome;
    }

    /*private void WriteSome(TileInfo target)
    {
        Debug.Log("set new target");
    }*/

    private void StartTargeting()
    {
        if (_playerState.IsAnyActionsAllowed())
        {
            StartCoroutine(Targeting());
        }
    }

    private IEnumerator Targeting()
    {

        while (_playerState.IsAnyActionsAllowed())
        {
            Vector3 actionDir = new Vector3(CustomInput.rightInput.x, 0f, CustomInput.rightInput.y);
            TileInfo targetTile = TileManagment.GetTile(_playerState.currentTile.tilePosition, actionDir, _playerState.currentAction.distance);

            if (IsTargetingAllowed(targetTile))
            {
                if (targetTile != _playerState.currentActionTarget)
                {
                    _playerState.currentActionTarget = targetTile;
                    OnFoundTarget?.Invoke(_playerState.currentActionTarget, _playerState.currentAction.actionType);
                }
            }
            else
            {
                OnLostTarget?.Invoke();
                _playerState.currentActionTarget = null;
            }
            yield return new WaitForFixedUpdate();
        }
        FailedTargeting();
    }

    private bool IsTargetingAllowed(TileInfo targetTile)
    {
        if (targetTile == null)
        {
            return false;
        }
        return targetTile != _playerState.currentTile; 
    }

    private void FailedTargeting()
    {
        OnLostTarget?.Invoke();
        StopAllCoroutines();
        _playerState.currentActionTarget = null;
    } 
}
