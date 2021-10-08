using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TileMovement))]
//[RequireComponent(typeof(PlayerActionManager))]
public class PlayerState : MonoBehaviour
{
    public ControlType controlType = ControlType.Player;
    public TileOwner ownerIndex = TileOwner.Ariost;
    public CharacterState prevState = CharacterState.Idle;    
    public CharacterState currentState = CharacterState.Idle;
    //public ActionType currentSubState = ActionType.None;

    public TileInfo currentTile;
    public TileInfo targetMoveTile;
    public TileInfo currentActionTarget;
    
    public PlayerAction[] fireActions;
    
    public PlayerAction defaultAction;
    public PlayerAction currentAction;

    public Action OnInitializied;
    public Action<CharacterState> OnCharStateChanged;
    public Action<ActionType> OnSubStateChanged;
    public Action OnActionChanged;
    public Action OnDefaultAction;
    public Action OnActionInterrupt;

    public Action OnDeath, OnRes;

    public List<PlayerState> enemies;

    private bool isInitialized = false;

    private void Awake()
    {
        TileManagment.OnInitialized += SetStartParams;
        CharSpawner.OnPlayerSpawned += ResetEnemies;
       // DeathChecker.OnPlayerDeathPermanent += ResetEnemies;
        OnCharStateChanged += OnStateChanged;
    }

    private void Start()
    {
        if (!isInitialized)
        {
            SetStartParams();
        }

        //Debug.Log(TileManagment.GetTileAlt(transform.position));
    }

    public void ResetEnemies()
    {
        enemies.Clear();
        enemies = SetEnemies();
    }

    private void OnStateChanged(CharacterState newState)
    {
        switch (newState)
        {
            case CharacterState.Idle:
                currentAction = defaultAction;
                if (prevState == CharacterState.Capture)
                {
                    OnActionInterrupt?.Invoke();
                }
                else 
                {
                    OnDefaultAction?.Invoke();
                }                
                break;
            case CharacterState.Move:
                currentAction = defaultAction;
                OnActionInterrupt?.Invoke();
                break;
        }
    }

    private void OnEnable()
    {
        currentAction = defaultAction;
    }

    public void SetNewState(CharacterState newState)
    {
        if (currentState != newState)
        {
            prevState = currentState;
            currentState = newState;
            /*if (newState == CharacterState.Action)
            {
                currentSubState = currentAction.actionType;
            }*/
            OnCharStateChanged?.Invoke(newState);            
        }
    }

    /*public void SetNewCharSubState(ActionType newActionType)
    {
        if (currentSubState != newActionType)
        {
            currentSubState = newActionType;
            OnSubStateChanged?.Invoke(newActionType);
        }
    }*/

    public void SetCurrentAction(PlayerAction newAction)
    {
        if (currentAction != newAction)
        {
            currentAction = newAction;
            OnActionChanged?.Invoke();
        }
    }

    public void SetStartParams()
    {
        currentTile = TileManagment.GetTile(transform.position);
        currentTile.canMove = false;
        //currentState = CharacterState.Idle;
        SetNewState(CharacterState.Idle);
        
        int fireIndx = controlType != ControlType.Player ? Random.Range(0, fireActions.Length) : PlayerPrefs.GetInt("fireMode");
        
        defaultAction = fireActions[fireIndx];
        
        currentAction = defaultAction;
        currentActionTarget = null;
        enemies = SetEnemies();
        //Debug.Log("player state init");
        OnInitializied?.Invoke();
        isInitialized = true;
    }

    private List<PlayerState> SetEnemies()
    {
        List<PlayerState> players = GameManager.activePlayers;
        List<PlayerState> enemies = new List<PlayerState>();
        foreach (PlayerState player in players)
        {
            if (player.gameObject.name != gameObject.name)
            {
                enemies.Add(player);
            }
        }
        return enemies;
    }

    public bool IsAnyActionsAllowed()
    {
        return currentTile.tileOwnerIndex == ownerIndex && currentState == CharacterState.Idle;
    }

    public void SetDead()
    {
        currentTile.canMove = true;
        if (targetMoveTile)
        {
            targetMoveTile.canMove = true;
        }
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
    public void SetAlive(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        gameObject.SetActive(true);
        SetStartParams();
        OnRes?.Invoke();        
    }
}

public enum CharacterState
{
    Idle,
    Capture,
    Move,
    Action,
    Frozen,
    /*Attack,
    Build,
    TreeAttack,*/
    Dead
}

public enum ControlType
{
    Player,
    AI
}

public enum ActionType
{
    None,
    Attack,
    Build,
    TreeAttack,
    SuperJump
}
