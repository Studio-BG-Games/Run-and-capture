using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTriggerSystem : MonoBehaviour
{
    public int autoattackDistance = 3;

    private PlayerState _playerState;   
    
    public Action OnActionEnd;

    private float _actionProgress;
    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();        

        if (_playerState.controlType == ControlType.Player)
        {
            CustomInput.OnTouchUp += TryToTriggerCurrentAction;
            CustomInput.OnTouchUp += TryToAutoAttack;
        }

    }

    private void TryToAutoAttack()
    {
        PlayerState targetPlayer = null;
        Debug.Log("auto ");
        foreach(var enemy in _playerState.enemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }
            if (Vector3.Distance(_playerState.currentTile.tilePosition, enemy.transform.position) <= 1.1f*autoattackDistance*TileManagment.tileOffset)
            {
                targetPlayer = enemy;
                //Debug.Log("I see " + enemy.gameObject.name);
                break;
            }
        }

        if (_playerState.IsAnyActionsAllowed() && _playerState.currentAction.actionType==ActionType.Attack && targetPlayer!=null)
        {
            if (!GetComponent<AttackEnergyController>().IsReady())
            {
                return;
            }
            StopAllCoroutines();
            List<TileInfo> adjacentTiles = TileManagment.GetAllAdjacentTiles(_playerState.currentTile);
            TileInfo closestTile = adjacentTiles[0];
            foreach (TileInfo tile in adjacentTiles)
            {
                float oldDist = Vector3.Distance(targetPlayer.transform.position, closestTile.tilePosition);
                float newDist = Vector3.Distance(targetPlayer.transform.position, tile.tilePosition);
                if (newDist < oldDist)
                {
                    closestTile = tile;
                }
            }
            _playerState.currentActionTarget = closestTile;
            DoAction(_playerState.currentAction);
        }
    }

    private void TryToTriggerCurrentAction()
    {
        
        if (_playerState.currentActionTarget && _playerState.IsAnyActionsAllowed())
        {
            //OnLostTarget?.Invoke();
            StopAllCoroutines();
            //Debug.Log("trigger current action");
            if (_playerState.currentAction.actionType == ActionType.Attack)
            {                
                if (!GetComponent<AttackEnergyController>().IsReady())
                {
                    return;
                }
            }
            DoAction(_playerState.currentAction);
        }
        /*else
        {
            failed attack
            OnActionEnd?.Invoke(ActionType.Attack, _playerState.currentState);
        }*/
    }

    private void DoAction(PlayerAction action)
    {
        if (!action.IsActionAllowed(_playerState.currentActionTarget, _playerState))
        {
            return;
        }       
        _playerState.SetNewState(CharacterState.Action);

        //Debug.Log("started state " + action.actionState);
        transform.LookAt(_playerState.currentActionTarget.tilePosition);
        action.StartActionOperations(_playerState.currentActionTarget, _playerState);
        StartCoroutine(WaitTillActionEnd(action));        
    }

    private IEnumerator WaitTillActionEnd(PlayerAction action)
    {
        TileInfo target = _playerState.currentActionTarget;
        float waitTime = action.duration;
        bool actionImpact = false;
        _actionProgress = 0f;
        float timer = 0f;
        while (_actionProgress < 1f)
        {
            if (_actionProgress > 0.7f && !actionImpact)
            {
                actionImpact = true;
                action.Impact(target, _playerState);
            }
            timer += Time.fixedDeltaTime;
            _actionProgress = timer / waitTime;

            yield return new WaitForFixedUpdate();
        }       
        FinalActionOperations(action);        
    }

    private void FinalActionOperations(PlayerAction action)
    {
        //OnActionEnd?.Invoke();
        //OnActionSuccess?.Invoke();
        _actionProgress = 0f;
        action.FinishActionOperations(_playerState);
        StopAllCoroutines();
        _playerState.SetNewState(CharacterState.Idle);               
       
    }

    public void TriggerAction(TileInfo target, PlayerAction action)
    {
        _playerState.currentActionTarget = target;
        _playerState.currentAction = action;
        TryToTriggerCurrentAction();
    }

    public float GetActionProgress()
    {
        return _actionProgress;
    }
        
}
