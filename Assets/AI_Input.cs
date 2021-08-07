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
    public Action OnTouchDown, OnTouchUp;
    public Action OnCurrentPathFinished;
    public List<PlayerState> enemies;
    public PlayerState _currentEnemy;

    private List<TileInfo> _currentFollowingPath;
    //public List<TileInfo> _testPath;

    private PlayerState _playerState;
    private TileMovement _tileMovement;    
    private PlayerActionManager _actionManager;    

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _tileMovement = GetComponent<TileMovement>();
        _actionManager = GetComponent<PlayerActionManager>();

        _actionManager.OnActionEnd += BackToPatrol;

        //_playerState.OnCharStateChanged += RecalculatePath;
        _tileMovement.OnFinishMovement += CheckState;
        _tileMovement.OnStartMovement += StopJoystick;
        _playerState.OnInitializied += StartPatrolBehaviour;

        OnCurrentPathFinished += StartPatrolBehaviour;

        SetEnemies();
    }

    private void BackToPatrol(ActionType arg1, CharacterState arg2)
    {
        StartPatrolBehaviour();
    }

    private void SetEnemies()
    {
        var allPlayers = FindObjectsOfType<PlayerState>();
        foreach (PlayerState player in allPlayers)
        {
            if (player.gameObject.name != gameObject.name)
            {
                enemies.Add(player);
            }            
        }
        //Debug.Log("added " + enemies.Count);
    }   

    private void StopJoystick(ActionType arg1, CharacterState arg2)
    {
        leftInput = Vector2.zero;
    }

    private void MoveTo(TileInfo tile)
    {
        if (_currentFollowingPath.Count > 0)
        {
            leftInput = TileManagment.GetJoystickDirection(_playerState.currentTile, tile);
        }
    }

    private void StartPatrolBehaviour()
    {
        botState = BotState.Patrol;
        TileInfo targetTile = TileManagment.GetRandomOtherTile(_playerState.ownerIndex);
        //Debug.Log("move to " + targetTile.name);
        var startTile = _playerState.currentTile;
        _currentFollowingPath = Pathfinding.FindPath(startTile, targetTile, TileManagment.levelTiles, TileManagment.tileOffset);
        Pathfinding.FindPath(_playerState.currentTile, TileManagment.GetTile(enemies[0].transform.position), TileManagment.levelTiles, TileManagment.tileOffset);
        MoveTo(_currentFollowingPath[1]);        
    }

    private IEnumerator CalmDown(float time)
    {
        yield return new WaitForSeconds(time);
        _currentEnemy = null;
        StartPatrolBehaviour();
        StopAllCoroutines();
    }

    private IEnumerator TryToAttack(float attackCoolDown)
    {
        while (_currentEnemy)
        {
            _actionManager.AttackEnemyOnTile(_currentEnemy.currentTile);
            yield return new WaitForSeconds(attackCoolDown);
        }
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
    

    private void CheckState(ActionType newType, CharacterState newState)
    {
        foreach (PlayerState enemy in enemies)
        {
            Debug.Log("Check near enemy");
            if (Vector3.Distance(enemy.transform.position, transform.position) <= TileManagment.tileOffset*1.1)
            {
                botState = BotState.Attack;
                _currentEnemy = enemy;
                break;
            }            
        }
        if (botState == BotState.Patrol)
        {
            foreach (PlayerState enemy in enemies)
            {                
                foreach (TileInfo tile in TileManagment.charTiles[(int)_playerState.ownerIndex])
                {
                    if ((enemy.transform.position - tile.tilePosition).magnitude < Mathf.Epsilon)
                    {
                        botState = BotState.Agressive;
                        _currentEnemy = enemy;
                        StartCoroutine(CalmDown(agressiveTime));                        
                    }                    
                }
                if (_currentEnemy != null)
                {
                    break;
                }
            }
        }

        SetBehaviour(botState);
    }

    private void AttackEnemy(PlayerState currentEnemy)
    {
        Debug.Log("attacking");
        leftInput = Vector2.zero;
        //_actionManager.AttackEnemyOnTile(currentEnemy.currentTile);
        StartCoroutine(TryToAttack(0.2f));
        StartCoroutine(CalmDown(attackTime));
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
        Debug.Log(adjacentTarget);
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
        if (_currentFollowingPath.Count > 0) //when stop movement, calculating path that begins from target tile
        {
            if (_playerState.currentTile == _currentFollowingPath[_currentFollowingPath.Count - 1])
            {
                OnCurrentPathFinished?.Invoke();
                return;
            }

            var endTile = _currentFollowingPath[_currentFollowingPath.Count - 1];
            if (!endTile.canMove)
            {
                endTile = TileManagment.GetRandomOtherTile(_playerState.ownerIndex);
                Debug.Log("changed target");
            }
            var currentTile = _playerState.currentTile;
            _currentFollowingPath.Clear();
            _currentFollowingPath = Pathfinding.FindPath(currentTile, endTile, TileManagment.levelTiles, TileManagment.tileOffset);
            MoveTo(_currentFollowingPath[1]);
        }
        
    }

    public enum BotState 
    {
        Patrol,
        Agressive,
        Attack,
        Dead
    }
   
}
