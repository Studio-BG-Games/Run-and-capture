using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileMovement))]
[RequireComponent(typeof(PlayerActionManager))]
public class PlayerState : MonoBehaviour
{
    public CharacterState currentState;
    public TileOwner ownerIndex = TileOwner.Ariost;
    public ActionType currentAction = ActionType.Attack;
    public ControlType controlType = ControlType.Player;


    public TileInfo currentTile;

    [SerializeField]
    private BuildingSelectionTool _selectionTool;
    private TileMovement _tileMovement;
    private PlayerActionManager _playerActions;

    public Action OnCaptureAllow, OnCaptureForbid, OnInitializied;
    public Action<CharacterState, ActionType> OnCharStateChanged;
    public Action<ActionType> OnActionChanged;

    private void Start()
    {
        SetStartParams();
        _tileMovement = GetComponent<TileMovement>();
        _playerActions = GetComponent<PlayerActionManager>();

        _tileMovement.OnStartMovement += SetNewState;
        _tileMovement.OnFinishMovement += SetNewState;

        _playerActions.OnActionStart += SetNewState;
        _playerActions.OnActionEnd += SetNewState;

        _selectionTool.OnBuildingSelected += OnBuildingSelected;

        //BuildManagment.OnBuildingSelected += SetPlacingBuildingsState;
        //BuildManagment.OnBuildBtnDeactivated += SetIdleState;

        //Debug.Log("We are in " +currentTile.name);

    }

    private void OnBuildingSelected()
    {
        SetNewState(ActionType.Build, currentState);
    }

    private void CaptureState(bool permission)
    {
        if (permission)
        {
            OnCaptureAllow?.Invoke();
        }
        else
        {
            OnCaptureForbid?.Invoke();
        }
    }

    private void SetNewState(ActionType actionType, CharacterState newState)
    {
        if (currentState != newState)
        {
            if (newState == CharacterState.Idle)
            {
                CaptureState(true);
            }
            else
            {
                CaptureState(false);
            }
            currentState = newState;
            OnCharStateChanged?.Invoke(newState, actionType);
        }
        if (currentAction != actionType )
        {            
            currentAction = actionType;
            OnActionChanged?.Invoke(actionType);
        }
        
    }    

    private void SetStartParams()
    {
        currentTile = TileManagment.GetTile(transform.position);
        currentState = CharacterState.Idle;

        OnInitializied?.Invoke();        
    }

    public bool IsAnyActionsAllowed()
    {
        return (currentTile.tileOwnerIndex == ownerIndex) && (currentState == CharacterState.Idle);
    }
}

public enum CharacterState 
{
    Idle,
    Move,    
    Action
}

public enum TileOwner
{
    Neutral = 0,
    Ariost = 1,
    Ragnar = 2,
    Emir = 3,
    Asvald = 4,
}

public enum ActionType
{
    Attack,
    Build,    
}

public enum ControlType
{
    Player,
    AI
}
