using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerBonusController : MonoBehaviour
{
    public int maxBonusCount = 3;

    public List<Bonus> attackBonuses;
    public List<Bonus> protectBonuses;

    public Action OnBonusesChanged;

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
}
