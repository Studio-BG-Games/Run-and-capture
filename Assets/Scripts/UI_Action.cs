using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Action : MonoBehaviour
{
    [SerializeField]
    private Transform _attackUI;
    [SerializeField]
    private Transform _buildUI;

    private PlayerActionManager _actionManager;

    void Start()
    {
        _actionManager = GetComponent<PlayerActionManager>();
        _actionManager.OnFoundTarget += OnStartTargeting;
        _actionManager.OnLostTarget += OnStopTargeting;

        SetInitialState();
    }

    private void SetInitialState()
    {
        _attackUI.gameObject.SetActive(false);
        _buildUI.gameObject.SetActive(false);
    }

    private void OnStopTargeting()
    {
        _actionManager.OnLostTarget -= OnStopTargeting;
        _attackUI.gameObject.SetActive(false);
        _buildUI.gameObject.SetActive(false);
        //Debug.Log("target null");
    }

    private void OnStartTargeting(TileInfo target, ActionType actionType)
    {
        _actionManager.OnLostTarget += OnStopTargeting;
        switch (actionType)
        {
            case ActionType.Attack:
                UpdateAttackUI(target.tilePosition, _attackUI);
                break;
            case ActionType.Build:
                UpdateBuildUI(target.tilePosition, _buildUI);
                break;
        }        
    }

    private void UpdateAttackUI(Vector3 targetPos, Transform attackUI)
    {
        if (targetPos != null)
        {
            attackUI.gameObject.SetActive(true);
            attackUI.LookAt(targetPos);
        }
    }

    private void UpdateBuildUI(Vector3 targetPos, Transform ui)
    {
        if (targetPos != null)
        {
            ui.gameObject.SetActive(true);
            ui.transform.position = targetPos;
        }
    }

    private void DisableAllBuildVisuals()
    {
        for (int i = 0; i < _buildUI.childCount; i++)
        {
            _buildUI.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetSelectedUI(int index)
    {
        DisableAllBuildVisuals();
        _buildUI.GetChild(index).gameObject.SetActive(true);
    }
}
