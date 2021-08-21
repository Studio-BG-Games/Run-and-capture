using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Bonus", menuName = "Bonuses/New Bonus")]
public class Bonus : ScriptableObject
{
    public BonusType bonusType = BonusType.Attack;
    public Sprite icon;
    public int bonusLevel;

    public void Use()
    {
        Debug.Log("Using " /*+ itemName*/);
        //FindObjectOfType<PlayerBonusController>().RemoveBonus(this);
    }

    /*public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }*/
}

public enum BonusType
{ 
    Attack,
    Defend
}

public enum BonusSpecification
{
    Tower,
    None
}



