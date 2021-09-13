using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlayerState))]
[RequireComponent(typeof(CaptureController))]
public class TileMovement : MonoBehaviour
{

    public float nextTileMoveTime = 0.8f;
    public int moveDistance = 1;

    public ParticleSystem moveVFX;

    private Vector3 _moveDir;

    private PlayerState _playerState;
    private AI_BotController _botController;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _botController = GetComponent<AI_BotController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (IsMoveCondition())
        {
            switch (_playerState.controlType)
            {
                case ControlType.Player:
                    _moveDir = new Vector3(CustomInput.leftInput.x, 0f, CustomInput.leftInput.y);
                    break;
                case ControlType.AI:
                    _moveDir = new Vector3(_botController.leftInput.x, 0f, _botController.leftInput.y);
                    break;

            }
            //_moveDir = new Vector3(CustomInput.leftInput.x, 0f, CustomInput.leftInput.y);
            if (_moveDir.magnitude > Mathf.Epsilon)
            {
                TileInfo targetMoveTile = TileManagment.GetTile(_playerState.currentTile.tilePosition, _moveDir, 1);
                if (IsMovementAllowed(targetMoveTile))
                {
                    MoveToTile(targetMoveTile);
                }
            }
        }      

    }

    private bool IsMovementAllowed(TileInfo tile)
    {
        if (tile)
        {
            bool canMoveToMyTile = (tile.tileOwnerIndex == _playerState.ownerIndex);
            bool canMoveToEnemyTile = (_playerState.currentTile.tileOwnerIndex == _playerState.ownerIndex);

            bool canMoveThroughBuilding = true;

            if (tile.buildingOnTile != null)
            {
                TrapObj trap = tile.buildingOnTile.GetComponent<TrapObj>();
                if (trap != null)
                {
                    canMoveThroughBuilding = trap.owner != _playerState.ownerIndex;
                }
            }

            return tile.canMove && (canMoveToMyTile || canMoveToEnemyTile) && canMoveThroughBuilding;
        }
        else
        {
            return false;
        }
        
    }

    private void MoveToTile(TileInfo targetMoveTile)
    {
        _playerState.SetNewState(CharacterState.Move);
        transform.DOMove(targetMoveTile.tilePosition, nextTileMoveTime).OnComplete(()=> FinishMovementActions(targetMoveTile));
        transform.LookAt(targetMoveTile.tilePosition);

        _playerState.targetMoveTile = targetMoveTile;
        _playerState.targetMoveTile.canMove = false;
        _playerState.currentTile.canMove = true;

        moveVFX.Play();
    }

    private bool IsMoveCondition()
    {
        return _playerState.currentState == CharacterState.Idle || _playerState.currentState == CharacterState.Capture;
    }
    private void FinishMovementActions( TileInfo currentTile)
    {
        //Debug.Log("finish movement");
        _playerState.currentTile = currentTile;
        _playerState.SetNewState(CharacterState.Idle);
    }    
}
