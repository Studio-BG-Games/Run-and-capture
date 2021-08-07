using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlayerState))]
public class TileMovement : MonoBehaviour
{

    public float nextTileMoveTime = 0.8f;
    public int moveDistance = 1;
    private Vector3 _moveDir;

    public Action<ActionType, CharacterState> OnFinishMovement;
    public Action<ActionType, CharacterState> OnStartMovement;

    private PlayerState _playerState;
    private AI_Input _AIInput;

    private void Start()
    {
        _playerState = GetComponent<PlayerState>();
        _AIInput = GetComponent<AI_Input>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_playerState.currentState != CharacterState.Idle)
            return;
        switch (_playerState.controlType)
        {
            case ControlType.Player:
                _moveDir = new Vector3(CustomInput.leftInput.x, 0f, CustomInput.leftInput.y);
                break;
            case ControlType.AI:
                _moveDir = new Vector3(_AIInput.leftInput.x, 0f, _AIInput.leftInput.y);
                break;

        }
        //_moveDir = new Vector3(CustomInput.leftInput.x, 0f, CustomInput.leftInput.y);
        if (_moveDir.magnitude > Mathf.Epsilon)
        {
            TileInfo targetMoveTile = TileManagment.GetTile(_playerState.currentTile.tilePosition, _moveDir, moveDistance);
            //Debug.Log("moving to " + targetMoveTile);
            if (IsMovementAllowed(targetMoveTile))
            {
                //Debug.Log(IsMovementAllowed(targetMoveTile));
                MoveToTile(targetMoveTile);
            }
        }

    }

    private bool IsMovementAllowed(TileInfo tile)
    {
        if (tile)
        {
            bool canMoveToMyTile = (tile.tileOwnerIndex == _playerState.ownerIndex);
            bool canMoveToEnemyTile = (_playerState.currentTile.tileOwnerIndex == _playerState.ownerIndex);
            

            return tile.canMove && (canMoveToMyTile || canMoveToEnemyTile);
        }
        else
        {
            //Debug.Log("moving " + gameObject.name);
            return false;
        }
        
    }

    private void MoveToTile(TileInfo targetMoveTile)
    {
        OnStartMovement?.Invoke(ActionType.Attack, CharacterState.Move);
        _playerState.currentTile.canMove = true;
        targetMoveTile.canMove = false;
        transform.DOMove(targetMoveTile.tilePosition, nextTileMoveTime).OnComplete(()=> FinishMovementActions(targetMoveTile));
        transform.LookAt(targetMoveTile.tilePosition);
    }
    

    private void FinishMovementActions( TileInfo currentTile)
    {
        _playerState.currentTile = currentTile;
        //_playerState.currentTile.canMove = false;
        OnFinishMovement?.Invoke(ActionType.Attack, CharacterState.Idle);
    }    
}
