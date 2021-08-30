using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BonusUI : MonoBehaviour
{
    public Transform attackBonusParent;
    public Transform protectBonusParent;   

    public PlayerBonusController bonusController;  

    public BonusSlot selectedSlot;

    BonusSlot[] attackSlots;
    BonusSlot[] protectSlots;

    public Action<Bonus> OnBonusSelected;
    

    private void Awake()
    {
        attackSlots = attackBonusParent.GetComponentsInChildren<BonusSlot>();
        protectSlots = protectBonusParent.GetComponentsInChildren<BonusSlot>();

        bonusController.OnBonusesChanged += UpdateUI;
    }    

    private void UpdateUI()
    {
        selectedSlot = null;
        for (int i = 0; i < attackSlots.Length; i++)
        {
            if (i < bonusController.attackBonuses.Count)
            {
                attackSlots[i].Additem(bonusController.attackBonuses[i]);
            }
            else
            {
                attackSlots[i].ClearSlot();
            }
        }

        for (int i = 0; i < protectSlots.Length; i++)
        {
            if (i < bonusController.protectBonuses.Count)
            {
                protectSlots[i].Additem(bonusController.protectBonuses[i]);
            }
            else
            {
                protectSlots[i].ClearSlot();
            }
        }
    }

    public void SetSelectedSlot(BonusSlot selection)
    {
        if (selectedSlot != selection)
        {
            selectedSlot = selection;
            var bonus = selectedSlot.GetItem();
            bonusController.SetupCurrentBonus(bonus);
            OnBonusSelected?.Invoke(bonus);
        }
        
    }
}
