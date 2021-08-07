using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerState))]
public class CaptureController : MonoBehaviour
{

    public float neutralCaptureTime = 3f, enemyCaptureTime = 5f, fastCaptureTime = 0f;

    private TileOwner _ownerIndex;
    private TileManagment _tileManagment;
    public PlayerState _playerState;

    private float _captureProgress= 0f;

    public Action OnCaptureStart, OnCaptureFailed;
    public Action<TileInfo, float> OnCaptureEnd;

    private IEnumerator _currentCoroutine;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _tileManagment = FindObjectOfType<TileManagment>();

        _playerState.OnInitializied += CaptureStartTile;
        _playerState.OnCaptureAllow += TryToCaptureTile;
        _playerState.OnCaptureForbid += StopCapturingTile;

        OnCaptureEnd += CaptureTile;
    }
    

    private void CaptureStartTile()
    {
        //Debug.Log("start tile cap");
        _ownerIndex = _playerState.ownerIndex;
        _tileManagment.ChangeTileOwner(_playerState.currentTile, _ownerIndex);
        _playerState.currentTile.canMove = false;

    }

    private void StopCapturingTile()
    {
        OnCaptureFailed?.Invoke();
        _captureProgress = 0f;
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    private void TryToCaptureTile()
    {
        TileInfo tile = _playerState.currentTile;
        //Debug.Log("Try to capture " + tile.name);
        if(_ownerIndex != tile.tileOwnerIndex)
        {
            if (tile.whoCanEasyGetTile != _ownerIndex)
            {
                if (tile.tileOwnerIndex == TileOwner.Neutral)
                {
                    _currentCoroutine = Capturing(tile, neutralCaptureTime);
                    StartCoroutine(_currentCoroutine);
                }
                else
                {
                    _currentCoroutine = Capturing(tile, enemyCaptureTime);
                    StartCoroutine(_currentCoroutine);
                }
            }
            else
            {
                CaptureTile(tile, fastCaptureTime);
            }
            
        }
    }

    private void CaptureTile(TileInfo tile, float captureTime)
    {
        _tileManagment.ChangeTileOwner(tile, _ownerIndex);
        //Debug.Log("Captured " + tile.name);
    }

    private IEnumerator Capturing(TileInfo tile, float captureTime)
    {
        OnCaptureStart?.Invoke();

        _captureProgress = 0f;
        float captureTimer = 0f;
        while (_captureProgress < 1f)
        {
            captureTimer += Time.fixedDeltaTime;
            _captureProgress = captureTimer / captureTime;
            yield return new WaitForFixedUpdate();
        }
        _captureProgress = 0f;

        OnCaptureEnd?.Invoke(tile, captureTime);
        StopCoroutine(_currentCoroutine);
    }

    public float GetCaptureProgress()
    {
        return _captureProgress;
    }
}


