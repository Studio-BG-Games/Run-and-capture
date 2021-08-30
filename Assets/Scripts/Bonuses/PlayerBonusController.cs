using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerBonusController : MonoBehaviour
{
    public int maxBonusCount = 3;

    public List<Bonus> attackBonuses;
    public List<Bonus> protectBonuses;

    public Bonus currentSelectedBonus;
    public Action OnBonusesChanged;

    private PlayerState _playerState;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        _playerState.OnDefaultAction += RemoveCurrentBonus;
        _playerState.OnActionInterrupt += ResetSelected;
        _playerState.OnDeath += RemoveAll;
    }

    private void ResetSelected()
    {
        if (currentSelectedBonus != null)
        {            
            currentSelectedBonus = null;
            OnBonusesChanged?.Invoke();
        }
    }

    private void RemoveCurrentBonus()
    {
        if (currentSelectedBonus != null)
        {
            RemoveBonus(currentSelectedBonus);
            currentSelectedBonus = null;
        }
    }
    

    public bool AddBonusToPlayer(Bonus bonus)
    {
        if (bonus.bonusType == BonusType.Attack)
        {
            if (attackBonuses.Count < maxBonusCount)
            {
                attackBonuses.Add(bonus);
                OnBonusesChanged?.Invoke();
                return true;
            }
            else 
            {
                return false;
            }
            
        }
        else 
        {
            if (protectBonuses.Count < maxBonusCount)
            {
                protectBonuses.Add(bonus);
                OnBonusesChanged?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void RemoveBonus(Bonus bonus)
    {
        if (protectBonuses.Contains(bonus))
        {
            protectBonuses.Remove(bonus);
        }
        else
        {
            attackBonuses.Remove(bonus);
        }
        OnBonusesChanged?.Invoke();

    }

    public void RemoveAll()
    {
        protectBonuses.Clear();
        attackBonuses.Clear();
        OnBonusesChanged?.Invoke();
    }

    public void SetupCurrentBonus(Bonus newBonus)
    {
        if (currentSelectedBonus != newBonus)
        {
            currentSelectedBonus = newBonus;
            _playerState.SetCurrentAction(currentSelectedBonus.bonusAction);
        }
    }
}
