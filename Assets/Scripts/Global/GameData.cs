using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameData : MonoBehaviour
{
    public static int coins = 0;
    public static int playerLevel = 5;

    public static Action OnCoinsCollected;
    public static void AddCoin(int amount)
    {
        if (amount > 0)
        {
            coins += amount;
            OnCoinsCollected?.Invoke();
        }
    }
    
}
