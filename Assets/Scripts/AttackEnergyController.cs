using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackEnergyController : MonoBehaviour
{
    private float maxAttackEnergy;
    private float attackResetTime;
    private float attackCost;

    private float attackEnergy;
    private float _silenceTime = 2f;
    private float _lastAttackTime = 0f;
    private bool _isReady = true;
    private bool _isCharging = true;

    public Action OnEnergyLow;
    public Action<float, float> OnAttackEnergyChanged;

    private PlayerState _playerState;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _playerState.OnCharStateChanged += OnPlayerAttack;
        
        maxAttackEnergy = _playerState.defaultAction.maxAttackEnergy;
        attackResetTime = _playerState.defaultAction.attackResetTime;
        attackCost = _playerState.defaultAction.attackCost;
    }

    private void OnPlayerAttack(CharacterState newState)
    {
        if (newState != CharacterState.Action || _playerState.currentAction.actionType!=ActionType.Attack)
        {
            return;
        }
        _lastAttackTime = Time.time;
        _isCharging = false;
        ReduceAttackEnergy();
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        attackEnergy = maxAttackEnergy;
        _isCharging = false;
        _isReady = true;
        OnAttackEnergyChanged?.Invoke(attackEnergy, maxAttackEnergy);
        StopAllCoroutines();
    }

    private void Update()
    {
        CheckAttackAllowance();
    }

    private void ReduceAttackEnergy()
    {
        
        attackEnergy -= attackCost;
        OnAttackEnergyChanged?.Invoke(attackEnergy, maxAttackEnergy);
        if (attackEnergy < attackCost)
        {
            _isReady = false;
            OnEnergyLow?.Invoke();
        }
    }

    private void CheckAttackAllowance()
    {
        if (Time.time > _lastAttackTime + _silenceTime)
        {
            if (!_isCharging && attackEnergy < maxAttackEnergy)
            {
                StartCoroutine(FillAttackEnergy(attackResetTime));
            }
        }

        _isReady = attackEnergy > attackCost;
    }

    private IEnumerator FillAttackEnergy(float fillTime)
    {        

        _isCharging = true;
        float timer = 0f;
        float currentAttackEnergy = attackEnergy;
        while (timer < fillTime)
        {
            float fillProgress = timer / fillTime;
            timer += Time.fixedDeltaTime;
            attackEnergy = currentAttackEnergy + fillProgress * attackCost;
            OnAttackEnergyChanged?.Invoke(attackEnergy, maxAttackEnergy);

            if (attackEnergy > maxAttackEnergy)
            {
                attackEnergy = maxAttackEnergy;
                OnAttackEnergyChanged?.Invoke(attackEnergy, maxAttackEnergy);
                break;
            }

            yield return new WaitForFixedUpdate();
        }
        _isCharging = false;        
    }

    public bool IsReady()
    {
        return _isReady;
    }
}
