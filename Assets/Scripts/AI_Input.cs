using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI_Input : MonoBehaviour
{
    public BotState botState = BotState.Patrol;
    public Vector2 leftInput, rightInput;

    public float agressiveTime = 5f;
    public float attackTime = 2f;
    public float updateBehaviourIn = 1f;
    public float neutralCapDistance = 20f;
    public float detectPlayerDistance = 4f;
    public Action OnTouchDown, OnTouchUp;
    public Action OnCurrentPathFinished, OnAttack;    
    public PlayerState _currentEnemy;

    public List<TileInfo> _currentFollowingPath;    

    private PlayerState _playerState;
    private TileMovement _tileMovement;    
    private PlayerActionManager _actionManager;
    private Attack _attack;

    private Vector3 _startBotPoint;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _tileMovement = GetComponent<TileMovement>();
        _actionManager = GetComponent<PlayerActionManager>();
        _attack = GetComponent<Attack>();

        _actionManager.OnActionSuccess += StartPatrolBehaviour;
        _actionManager.OnActionStart += OnActionStart;
        
        _tileMovement.OnStartMovement += StopJoystick;
        _playerState.OnInitializied += ActivateAI;

        OnCurrentPathFinished += StartPatrolBehaviour;

        _playerState.OnDeath += StopAllActions;

        _attack.OnEnergyLow += StartPatrolBehaviour;

        _startBotPoint = transform.position;        

    }

    private void StopAllActions()
    {
        botState = BotState.Patrol;
        StopAllCoroutines();
    }    

    private IEnumerator CheckBotState(float updateTime)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1f));

        while (true)
        {
            CheckState();
            yield return new WaitForSeconds(updateTime);
        }
    }

    /*private void OnEnable()
    {
        Debug.Log("enable");
        //StartPatrolBehaviour();
        StartCoroutine(CheckBotState(updateBehaviourIn));        
    } */   

    private void OnActionStart(ActionType arg1, CharacterState arg2)
    {
        _currentEnemy = null;
    }

    private void ActivateAI()
    {
        Debug.Log("back to patrol");        
        StartPatrolBehaviour();
        StartCoroutine(CheckBotState(updateBehaviourIn));
    }
    

    private void StopJoystick(ActionType arg1, CharacterState arg2)
    {
        leftInput = Vector2.zero;
    }

    private void MoveToNearTile(TileInfo tile)
    {
        if (_currentFollowingPath.Count > 0)
        {
            leftInput = TileManagment.GetDirection(_playerState.currentTile, tile);
        }
    }

    private void StartPatrolBehaviour()
    {
        //Debug.Log("start Patrol");
        _currentEnemy = null;
        botState = BotState.Patrol;
        TileInfo targetTile = TileManagment.GetNearestOtherTile(_playerState.currentTile, _playerState.ownerIndex, neutralCapDistance, _startBotPoint);        
        var startTile = _playerState.currentTile;
        _currentFollowingPath = Pathfinding.FindPath(startTile, targetTile, TileManagment.levelTiles, TileManagment.tileOffset);
        if (_currentFollowingPath == null)
        {
            StartPatrolBehaviour();
            return;
        }
    }   

    private void CheckState()
    {
        Debug.Log("Check state");        
        if (botState != BotState.Attack && _attack.IsReady())
        {
            foreach (PlayerState enemy in _playerState.enemies)
            {
                if (!enemy.gameObject.activeSelf)
                {
                    continue;
                }
                if (Vector3.Distance(enemy.transform.position, transform.position) <= TileManagment.tileOffset * 1.1f)
                {
                    //Debug.Log("attack state");
                    botState = BotState.Attack;
                    _currentEnemy = enemy;
                    break;
                }
            }
        }        
        if (botState == BotState.Patrol && _attack.IsReady())
        {            
            foreach (PlayerState enemy in _playerState.enemies)
            {
                if (!enemy.gameObject.activeSelf)
                {
                    continue;
                }
                foreach (TileInfo tile in TileManagment.charTiles[(int)_playerState.ownerIndex])
                {
                    if ((enemy.transform.position - tile.tilePosition).magnitude < Mathf.Epsilon)
                    {
                        botState = BotState.Agressive;
                        _currentEnemy = enemy;
                        StartCoroutine(CalmDown(agressiveTime));
                        //Debug.Log("aggressive state");
                    }
                    else if ((enemy.transform.position - transform.position).magnitude < detectPlayerDistance)
                    {
                        botState = BotState.Agressive;
                        _currentEnemy = enemy;
                        StartCoroutine(CalmDown(agressiveTime));
                        //Debug.Log("aggressive state");
                    }
                }
                if (_currentEnemy != null)
                {
                    //Debug.Log("found enemy");
                    break;
                }
            }
        }

        SetBehaviour(botState);
        
    }

    private void SetBehaviour(BotState state)
    {
        switch (state)
        {
            case BotState.Patrol:
                MoveToNextPoint();
                break;
            case BotState.Agressive:
                MoveToEnemy(_currentEnemy);
                break;
            case BotState.Attack:
                AttackEnemy(_currentEnemy);
                break;
        }
    }

    private void AttackEnemy(PlayerState currentEnemy)
    {
        if (currentEnemy)
        {
            //Debug.Log("startAttack");
            leftInput = Vector2.zero;
            _currentFollowingPath.Clear();            
            _actionManager.AttackEnemyOnTile(_currentEnemy.currentTile);
        }
    }

    private void MoveToEnemy(PlayerState currentEnemy)
    {
        List<TileInfo> allAdjacentTilesToTarget = TileManagment.GetAllAdjacentTiles(currentEnemy.currentTile);
        TileInfo adjacentTarget = allAdjacentTilesToTarget[0];
        foreach (var tile in allAdjacentTilesToTarget)
        {
            if (tile.canMove)
            {
                adjacentTarget = tile;
                break;
            }
        }
        TileInfo currentPos = TileManagment.GetTile(transform.position);        
        RecalculatePath(currentPos, adjacentTarget);
        if (_currentFollowingPath == null)
        {
            StartPatrolBehaviour();
            return;
        }
        MoveToNextPoint();
    }

    private void RecalculatePath(TileInfo curentPos, TileInfo target)
    {
        _currentFollowingPath.Clear();
        _currentFollowingPath = Pathfinding.FindPath(curentPos, target, TileManagment.levelTiles, TileManagment.tileOffset);        
    }

    private void MoveToNextPoint()
    {
        //Debug.Log("try to move next point");
        if (_currentFollowingPath != null) //when stop movement, calculating path that begins from target tile
        {
            if (_playerState.currentTile.tileOwnerIndex == _playerState.ownerIndex)
            {
                if (_playerState.currentTile == _currentFollowingPath[_currentFollowingPath.Count - 1])
                {
                    OnCurrentPathFinished?.Invoke();
                    return;
                }

                var endTile = _currentFollowingPath[_currentFollowingPath.Count - 1];
                if (!endTile.canMove)
                {
                    endTile = TileManagment.GetNearestOtherTile(_playerState.currentTile, _playerState.ownerIndex, neutralCapDistance, _startBotPoint);
                    
                }
                var currentTile = _playerState.currentTile;
                _currentFollowingPath.Clear();
                _currentFollowingPath = Pathfinding.FindPath(currentTile, endTile, TileManagment.levelTiles, TileManagment.tileOffset);
                MoveToNearTile(_currentFollowingPath[1]);
                
            }

        }
        else if (_playerState.currentTile.tileOwnerIndex == _playerState.ownerIndex)
        {
            OnCurrentPathFinished?.Invoke();
        }

    }

    private IEnumerator CalmDown(float time)
    {
        yield return new WaitForSeconds(time);
        _currentEnemy = null;
        StartPatrolBehaviour();
        //StopAllCoroutines();
    }
    

    public enum BotState 
    {
        Patrol,
        Agressive,
        Attack,
        Dead
    }
    
}
