using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerState))]
public class PlayerActionManager : MonoBehaviour
{
    public PlayerAction attackAction, buildAction;

    public Action<ActionType, CharacterState> OnActionStart, OnActionEnd;

    public Action<TileInfo, ActionType> OnFoundTarget;
    public Action OnLostTarget, OnActionSuccess;

    private PlayerState _playerState;
    private PlayerAction _currentAction;    

    private TileInfo _target;

    private float _actionProgress;   

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _playerState.OnActionChanged += SetNewCurrentAction;

        if (_playerState.controlType == ControlType.Player)
        {
            CustomInput.OnTouchDown += StartTargeting;
            CustomInput.OnTouchUp += StopTargeting;
        }       

        SetNewCurrentAction(ActionType.Attack);
    }   

    private void SetNewCurrentAction(ActionType newAction)
    {
        switch (newAction)
        {
            case ActionType.Attack:
                _currentAction = attackAction;
                break;
            case ActionType.Build:
                _currentAction = buildAction;
                break;
        }
    }    
    
    private void StartTargeting()
    {
        if (_playerState.IsAnyActionsAllowed())
        {
            //_currentCoroutine = Targeting();
            //StartCoroutine(_currentCoroutine);
            StartCoroutine(Targeting());
        }
    }    

    private IEnumerator Targeting()
    {

        while (_playerState.IsAnyActionsAllowed())
        {
            Vector3 actionDir = new Vector3(CustomInput.rightInput.x, 0f, CustomInput.rightInput.y);
            TileInfo targetTile = TileManagment.GetTile(_playerState.currentTile.tilePosition, actionDir, _currentAction.distance);
            
            if (_currentAction.IsActionAllowed(targetTile, _playerState))
            {
                if (targetTile != _target)
                {                   
                    OnFoundTarget?.Invoke(targetTile, _currentAction.actionType);
                    _target = targetTile;                    
                }
            }
            else
            {
                OnLostTarget?.Invoke();
                _target = null;
            }
            yield return new WaitForFixedUpdate();
        }
        FailedTargeting();
    }

    private void FailedTargeting()
    {
        OnLostTarget?.Invoke();
        //StopCoroutine(_currentCoroutine);
        StopAllCoroutines();        
        _target = null;
    }

    private void StopTargeting()
    {
        if (_target != null && _playerState.IsAnyActionsAllowed() /*&& _currentCoroutine!=null*/)
        {
            OnLostTarget?.Invoke();
            //StopCoroutine(_currentCoroutine); //stop targeting
            //_currentCoroutine = null;
            StopAllCoroutines();
            DoAction(_currentAction);
        }
        else 
        {
            //Debug.Log("failed attack");
            OnActionEnd?.Invoke(ActionType.Attack, _playerState.currentState);
        }
    }

    private void DoAction(PlayerAction action)
    {
        if (!action.IsActionAllowed(_target, _playerState))
        {
            return;
        }
        OnActionStart?.Invoke(action.actionType, CharacterState.Action);        
        transform.LookAt(_target.tilePosition);
        action.StartActionOperations(_target);
        //_currentCoroutine = WaitTillActionEnd(action);
        //StartCoroutine(_currentCoroutine);
        StartCoroutine(WaitTillActionEnd(action));
    }

    private IEnumerator WaitTillActionEnd(PlayerAction action)
    {
        //Debug.Log("started cor");
        float time = action.duration;
        bool actionImpact = false;
        _actionProgress = 0f;
        float timer = 0f;
        while (_actionProgress < 1f)
        {
            if (_actionProgress > 0.7f && !actionImpact)
            {
                actionImpact = true;
                action.Impact(_target, _playerState.currentTile, _playerState.ownerIndex);
            }
            timer += Time.fixedDeltaTime;
            _actionProgress = timer / time;
            yield return new WaitForFixedUpdate();
        }
        _actionProgress = 0f;
        FinalActionOperations(action);
        //StopCoroutine(_currentCoroutine);
        //_currentCoroutine = null;
        StopAllCoroutines();
    }

    private void FinalActionOperations(PlayerAction action)
    {        
         OnActionEnd?.Invoke(ActionType.Attack, CharacterState.Idle);
        OnActionSuccess?.Invoke();
        _target = null;
        //Debug.Log(action.actionType + " ended");        
    }

    public float GetProgress()
    {
        return _actionProgress;
    }

    public void AttackEnemyOnTile(TileInfo target)
    {
        _target = target;        
        StopTargeting();
    }


}



