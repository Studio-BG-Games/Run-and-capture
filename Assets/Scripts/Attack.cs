using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : PlayerAction
{
    public GameObject standartAttackPref;
    public GameObject standartAttackGroundImpact;

    public float maxAttackEnergy = 3f;
    public float attackResetTime = 3f;
    public float attackCost = 1f;

    public UI_Quantity UI_Energy;

    private float attackEnergy;
    private float _silenceTime = 2f;
    private float _lastAttackTime = 0f;
    private bool _isReady = true;
    private bool _isCharging = true;

    public Action OnEnergyLow;

    private void OnEnable()
    {
        attackEnergy = maxAttackEnergy;
        _isCharging = false;
        _isReady = true;
        UI_Energy.UpdateBar(attackEnergy, maxAttackEnergy);

        StopAllCoroutines();
    }

    private void Update()
    {
        CheckAttackAllowance();
    }
    public override bool IsActionAllowed(TileInfo targetTile, PlayerState playerState)
    {
        if (!targetTile)
            return false;
        bool permission = base.IsActionAllowed(targetTile, playerState);
        permission = permission && targetTile.canBeAttacked && _isReady;
        return permission;
    }

    public override void Impact(TileInfo targetTile, TileInfo currentTile, TileOwner owner)
    {
        base.Impact(targetTile, currentTile, owner);
        Vector3 direction = targetTile.tilePosition - currentTile.tilePosition;
        InitAttack(currentTile.tilePosition, direction, owner);
    }

    private void InitAttack(Vector3 startPosition, Vector3 direction, TileOwner projOwner)
    {
        var currentProjectile = Instantiate(standartAttackPref, startPosition, standartAttackPref.transform.rotation).GetComponent<ProjectileController>();
        currentProjectile.SetinitialParams(projOwner, direction, TileManagment.tileOffset);

        Instantiate(standartAttackGroundImpact, startPosition, standartAttackGroundImpact.transform.rotation);

        _lastAttackTime = Time.time;
        _isCharging = false;
        ReduceAttackEnergy();
        UI_Energy.UpdateBar(attackEnergy, maxAttackEnergy);

        StopAllCoroutines();
    }

    private void ReduceAttackEnergy()
    {
        attackEnergy-=attackCost;

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
        //Debug.Log(attackEnergy);

        _isCharging = true;
        float timer = 0f;
        float currentAttackEnergy = attackEnergy;
        while (timer < fillTime)
        {
            float fillProgress = timer / fillTime;
            timer += Time.fixedDeltaTime;
            attackEnergy = currentAttackEnergy + fillProgress * attackCost;
            UI_Energy.UpdateBar(attackEnergy, maxAttackEnergy);

            if (attackEnergy > maxAttackEnergy)
            {
                attackEnergy = maxAttackEnergy;
                UI_Energy.UpdateBar(attackEnergy, maxAttackEnergy);
                break;
            }

            yield return new WaitForFixedUpdate();
        }
        _isCharging = false;
        //Debug.Log(attackEnergy);
    }

    public bool IsReady()
    {
        return _isReady;        
    }
}
