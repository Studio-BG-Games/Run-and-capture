using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Bonus", menuName = "Bonuses/New Bonus")]
public class Bonus : ScriptableObject
{
    public BonusType bonusType = BonusType.Attack;
    public PlayerAction bonusAction;
    public Sprite icon;
    public int bonusLevel;    

}

public enum BonusType
{ 
    Attack,
    Defend
}




