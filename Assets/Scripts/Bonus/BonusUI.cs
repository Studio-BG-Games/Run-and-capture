using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusUI : MonoBehaviour
{
    public Transform attackBonusParent;
    public Transform protectBonusParent;
    //public GameObject inventoryUI;

    public PlayerBonusController bonusController;

    BonusSlot[] attackSlots;
    BonusSlot[] protectSlots;

    private void Start()
    {
        attackSlots = attackBonusParent.GetComponentsInChildren<BonusSlot>();
        protectSlots = protectBonusParent.GetComponentsInChildren<BonusSlot>();

        bonusController.OnBonusesChanged += UpdateUI;
    }

    /*private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            if (inventoryUI.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }*/

    private void UpdateUI()
    {
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
}
