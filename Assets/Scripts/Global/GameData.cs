using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameData : MonoBehaviour
{
    public static int coins = 0;
    public static int playerLevel = 5;
    public static int gameMaxPlayers = 2;

    public static bool isMusicAllowed = true;
    public static bool isSFXAllowed = true;

    private const string musKey = "Music settings";
    private const string sfxKey = "SFX settings";


    public static Action OnCoinsCollected;
    public static void AddCoin(int amount)
    {
        if (amount > 0)
        {
            coins += amount;
            OnCoinsCollected?.Invoke();
        }
    }

    public static void SaveSettings()
    {
        int musicValue = isMusicAllowed ? 1 : 0;
        int sfxValue = isSFXAllowed ? 1 : 0;
        PlayerPrefs.SetInt(musKey, musicValue);
        PlayerPrefs.SetInt(sfxKey, sfxValue);
    }

    public static void LoadSettings()
    {
        if (!PlayerPrefs.HasKey(musKey))
        {
            SaveSettings();
        }
        else 
        {
            isMusicAllowed = PlayerPrefs.GetInt(musKey) == 1 ? true : false;
            isSFXAllowed = PlayerPrefs.GetInt(sfxKey) == 1 ? true : false;
        }
    }

}
