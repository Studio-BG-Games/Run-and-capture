using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI_Input : MonoBehaviour
{
    public Vector2 leftInput, rightInput;

    public Action OnTouchDown, OnTouchUp;

    public List<TileInfo> _currentFollowingPath;
    public List<TileInfo> _testPath;

    private PlayerState _playerState;
    private TileMovement _tileMovement;

    private const int endIndex = 160;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _tileMovement = GetComponent<TileMovement>();
        //_playerState.OnCharStateChanged += RecalculatePath;
        _tileMovement.OnStartMovement += RecalculatePath;
        _playerState.OnInitializied += StartBehaviour;
    }

    private void StartBehaviour()
    {
        var endTile = TileManagment.levelTiles[endIndex];
        var startTile = _playerState.currentTile;
        _currentFollowingPath = Pathfinding.FindPath(startTile, endTile, TileManagment.tileOffset);
        _testPath = _currentFollowingPath;
    }

    private void Start()
    {
        
    }

    private void RecalculatePath(ActionType newType, CharacterState newState)
    {
        /*if (_currentFollowingPath.Count <= 0)
            return;*/
        /*if (_testPath.Count > 0)
        {
            var endTile = _testPath[_testPath.Count - 1];
            var currentTile = _testPath[1];
            //_currentFollowingPath.Clear();
            _testPath = Pathfinding.FindPath(currentTile, endTile, TileManagment.tileOffset);
            Debug.Log("recalculated, currentTile is " + currentTile.name);
        }*/
        
    }

    private void Update()
    {
        var cuurentTile = _playerState.currentTile;
        int nextTileIndex = _currentFollowingPath.IndexOf(cuurentTile)+1;
        if (nextTileIndex <= _currentFollowingPath.Count - 1)
        {
            leftInput = TileManagment.GetJoystickDirection(cuurentTile, _currentFollowingPath[nextTileIndex]);
        }
        else 
        {
            leftInput = Vector2.zero;
        }
    }
}
