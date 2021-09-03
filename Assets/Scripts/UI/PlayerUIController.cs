using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public float updateRate = 0.01f;
    [SerializeField]
    private Transform _attackUI;
    [SerializeField]
    private UI_Progress _progressUI;
    [SerializeField]
    private UI_Quantity healthUI;
    [SerializeField]
    private UI_Quantity attackEnergyUI;

    private PlayerState _playerState;
    private ActionTargetingSystem _targetingSystem;
    private ActionTriggerSystem _triggerSystem;
    private CaptureController _captureController;
    private HealthController _healthController;
    private AttackEnergyController _attackEnergy;

    private IEnumerator _progressUICoroutine;

    private delegate float GetProgress();
    private GetProgress GetActionProgress;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _targetingSystem = GetComponent<ActionTargetingSystem>();
        _triggerSystem = GetComponent<ActionTriggerSystem>();
        _captureController = GetComponent<CaptureController>();
        _healthController = GetComponent<HealthController>();
        _attackEnergy = GetComponent<AttackEnergyController>();


        _targetingSystem.OnFoundTarget += UpdateActionUI;
        _targetingSystem.OnLostTarget += StopUpdateAttackUI;        

        _playerState.OnCharStateChanged += StartUpdatingProgressUI;

        _healthController.OnHealthChanged += UpdateHealthUI;

        _attackEnergy.OnAttackEnergyChanged += UpdateEnergyUI;
    }

    private void UpdateActionUI(TileInfo target, ActionType actionType)
    {
        Vector3 targetPos = target.tilePosition;
        if (targetPos != null)
        {
            _attackUI.gameObject.SetActive(true);
            _attackUI.LookAt(targetPos);
        }
    }

    private void UpdateEnergyUI(float curEnergy, float maxEnergy)
    {        
        attackEnergyUI.UpdateBar(curEnergy, maxEnergy);
    }

    private void UpdateHealthUI(float curHealth, float maxHealth)
    {
        healthUI.UpdateBar(curHealth, maxHealth);
    }

    private void StartUpdatingProgressUI(CharacterState newState)
    {
        //Debug.Log("UPDATING UI");
        GetActionProgress = null;
        _progressUI.gameObject.SetActive(true);
        _progressUICoroutine = UpdateProgressBar(updateRate);
        string actionTypeText = "";
        switch (newState)
        {
            /*case CharacterState.Attack:
                actionTypeText = "Attack";
                GetActionProgress += GetCurrentActionProgress;                
                break;*/
            case CharacterState.Capture:
                actionTypeText = "Capturing...";
                GetActionProgress += GetCaptureProgress;                
                break;
            case CharacterState.Move:
                StopUpdateProgressUI();                
                return;
            case CharacterState.Idle:
                StopUpdateProgressUI();                
                return;
            case CharacterState.Action:
                actionTypeText = GetActionText(_playerState);
                GetActionProgress += GetCurrentActionProgress;
                break;
            /*case CharacterState.Build:
                actionTypeText = "Building...";
                GetActionProgress += GetCurrentActionProgress;                
                break;
            case CharacterState.TreeAttack:
                actionTypeText = "Attack";
                GetActionProgress += GetCurrentActionProgress;
                break;*/
            default:                
                return;
                
        }
        _progressUI.UpdateActionType(actionTypeText);
        StartCoroutine(_progressUICoroutine);
    }

    private string GetActionText(PlayerState player)
    {
        var charSubState = player.currentAction.actionType;
        string result = "";
        switch (charSubState)
        {
            case ActionType.Attack:
                result = "Attack";
                break;
            case ActionType.Build:
                result = "Build...";
                break;
            case ActionType.TreeAttack:
                result = "TreeAttack";
                break;
            case ActionType.SuperJump:
                result = "SuperJump";
                break;
        }
        return result;
    }

    private void StopUpdateProgressUI()
    {
        _progressUI.StopUpdateUI();

        if (_progressUICoroutine != null)
        {
            StopCoroutine(_progressUICoroutine);
            _progressUICoroutine = null;
        }
        _progressUI.gameObject.SetActive(false);

    }

    private float GetCurrentActionProgress()
    {        
        return _triggerSystem.GetActionProgress();
    }

    private float GetCaptureProgress()
    {
        return _captureController.GetCaptureProgress();
    }

    private IEnumerator UpdateProgressBar(float updateRate)
    {
        float currentActionProgress = 0f;
        while (true)
        {
            if (GetActionProgress != null)
            {
                currentActionProgress = GetActionProgress.Invoke();
            }
            _progressUI.UpdateUI(currentActionProgress);            
            yield return new WaitForSeconds(updateRate);
        }
        
    }

    private void StopUpdateAttackUI()
    {
        _attackUI.gameObject.SetActive(false);
    }
    

}
