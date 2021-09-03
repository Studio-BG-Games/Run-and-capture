using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerState))]
public class CaptureController : MonoBehaviour
{

    public float neutralCaptureTime = 3f, enemyCaptureTime = 5f/*, fastCaptureTime = 0f*/;

    [SerializeField]
    private GameObject capVFX;
    
    private PlayerState _playerState;

    private float _captureProgress= 0f;

    //public Action OnCaptureStart, OnCaptureFailed;
    //public Action<TileInfo, float> OnCaptureEnd;

    private IEnumerator _currentCoroutine;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _playerState.OnInitializied += CaptureStartTile;
        _playerState.OnCharStateChanged += CheckCapturing;

        //OnCaptureEnd += CaptureTile;
    }   

    private void CheckCapturing(CharacterState newState)
    {
       
        switch (newState)
        {
            case CharacterState.Idle:
                TryToCaptureTile();                
                break;
            case CharacterState.Move:
                StopCapturingTile();                
                break;
            default:
                return;
        }
    }

    private void TryToCaptureTile()
    {
        TileInfo tile = _playerState.currentTile;

        if (_playerState.ownerIndex != tile.tileOwnerIndex)
        {
            _playerState.SetNewState(CharacterState.Capture);

            if (tile.easyCaptureFor.Contains(_playerState.ownerIndex) || tile.easyCaptureForAll)
            {
                CaptureTile(tile);
            }
            else
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


        }
        else
        {
            TileManagment.SetPlayerTilesCapState(_playerState);
        }
    }    

    private void CaptureStartTile()
    {
        //Debug.Log("capStartTile");
        if (_playerState.currentTile.tileOwnerIndex != _playerState.ownerIndex)
        {
            CaptureTile(_playerState.currentTile);
        }        
    }

    private void StopCapturingTile()
    {
        if (_currentCoroutine != null)
        {
            //OnCaptureFailed?.Invoke();
            _captureProgress = 0f;            
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;            
        }
    }

    public void CaptureTile(TileInfo tile)
    {
        TileManagment.ChangeTileOwner(tile, _playerState);
        _playerState.SetNewState(CharacterState.Idle);
        if (capVFX != null)
        {
            Instantiate(capVFX, tile.tilePosition + capVFX.transform.position, capVFX.transform.rotation);
        }        
    }

    private IEnumerator Capturing(TileInfo tile, float captureTime)
    {
        //OnCaptureStart?.Invoke();        
        _captureProgress = 0f;
        float captureTimer = 0f;
        while (_captureProgress < 1f)
        {
            captureTimer += Time.fixedDeltaTime;
            _captureProgress = captureTimer / captureTime;            

            yield return new WaitForFixedUpdate();            
        }
        _captureProgress = 0f;

        //OnCaptureEnd?.Invoke(tile, captureTime);
        CaptureTile(tile);
        StopCapturingTile();
        //StopCoroutine(_currentCoroutine);
    }

    public float GetCaptureProgress()
    {
        return _captureProgress;
    }
}


