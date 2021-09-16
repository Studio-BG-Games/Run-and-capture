using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraSizeController : MonoBehaviour
{
    public float maxSize = 7.5f, camExpandSmoothness = 0.01f;
    public TileOwner trackingPlayer = TileOwner.Ariost;

    private float _startSize;

    [SerializeField] private CinemachineVirtualCamera _cam;

    private void Awake()
    {
        //TileManagment.OnAnyTileCaptured += UpdateCamSize;
        _startSize = _cam.m_Lens.OrthographicSize;
    }

    private void FixedUpdate()
    {
        UpdateCamSize();
    }

    private void UpdateCamSize(PlayerState player)
    {
        if (player.ownerIndex != trackingPlayer)
        {
            return;
        }
        int maxTilesNumber = TileManagment.levelTiles.Count;
        int playerTilesNumber = TileManagment.charTiles[(int)player.ownerIndex].Count;

        float camSizeDelta = maxSize - _startSize;

        _cam.m_Lens.OrthographicSize = _startSize + (float)playerTilesNumber / maxTilesNumber * camSizeDelta;
    }

    private void UpdateCamSize()
    {
        int maxTilesNumber = TileManagment.levelTiles.Count;
        int playerTilesNumber = TileManagment.charTiles[(int)trackingPlayer].Count;

        float camSizeDelta = maxSize - _startSize;

        float targetSize = _startSize + (float)playerTilesNumber / maxTilesNumber * camSizeDelta;

        _cam.m_Lens.OrthographicSize = Mathf.Lerp(_cam.m_Lens.OrthographicSize, targetSize, camExpandSmoothness);
    }
}
